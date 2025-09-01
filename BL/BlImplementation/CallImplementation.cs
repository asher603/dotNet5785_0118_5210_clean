namespace BlImplementation;
using BlApi;
using Helpers;
using BO;
using System.Collections.Generic;
using DO;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call callToAdd)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            CallManager.ValidateCallFieldsExceptAddress(callToAdd); // if invalid call fields, an exception will be thrown here

            DO.Call doCall = new DO.Call()
            {
                Id = callToAdd.Id,
                FullAddress = string.Empty, // set to string.Empty until we validate the address in validateAddressAndUpdateAddressAndCoordinatesForCallAsync and if valid we will update the call to have it
                CType = (DO.CallType)(int)callToAdd.CType,
                Description = callToAdd.Description ?? "",    
                Latitude = 0.0,  // set to 0.0 until we validate the new address and update the new Latitude in validateAddressAndUpdateAddressAndCoordinatesForCallAsync
                Longitude = 0.0,    // set to 0.0 until we validate the new address and update the new Longitude in validateAddressAndUpdateAddressAndCoordinatesForCallAsync
                Opening = AdminManager.Now,
                MaxTime = callToAdd.MaxTime
            };


            lock (AdminManager.BlMutex)
                _dal.Call.Create(doCall);

            CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation

            // after we done with adding the call except for the its address and coordinates, and also notified the observers, we can now update the address and coordinates

            // we can not identify the call we just created by its Id since we dont have this id, it is automaticly generated in the DL layer
            // therefore we will identify the call by its properties which we do have (from the doCall object)
            DO.Call? createdCall = _dal.Call.Read(c => c.Opening == doCall.Opening && c.CType == doCall.CType && c.Description == doCall.Description && c.MaxTime == doCall.MaxTime);

            if (createdCall == null)
                throw new BlDoesNotExistException("Failed to add call - the call was not created in the database.");

            // creating a new object of DO.Call wth the new address (unlike doCall which had the old address since we didnt know if the new address is valid when sending him in the update function)
            DO.Call createdCallWithNewAddress = createdCall with { FullAddress = callToAdd.FullAddress };

            // we dont need to wait for the results here, inside the ValidationAndCalculationOfNetRelatedDetailsAndSendingMailUponCallOpening we do wait for the validation and reciving of coordinates before sending the mail
            _ = CallManager.ValidationAndCalculationOfNetRelatedDetailsInAddedCallAndSendingMailUponCallOpening(createdCallWithNewAddress);
        }
        catch (BlInvalidInputException ex)
        {
            throw new BlInvalidInputException("Failed to add call", ex);
        }
    }

    public void CancelAssignment(int requestorId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        Tools.CancelAssignmentInTools(requestorId, assignmentId);
    }

    public void ChooseCall(int volunteerId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        CallManager.ChooseCallInManager(volunteerId, callId);
    }

    public void DeleteCall(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        if (!CallManager.canBeDeleted(callId))
            throw new BlDeletionImpossibleException("Can only delete a call with a status Open and doesn't have assignment");
        try
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Delete(callId);
        }
        catch(DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"The call with Id =  {callId} doesn't exist",ex);
        }
        CallManager.Observers.NotifyItemUpdated(callId);  // notify observers that the logical state of the BO.Call with that id has changed and may not match the current visual representation (this call is now deleted)
        CallManager.Observers.NotifyListUpdated();    // notify observers that the logical state of the lists of objects of call related BO entitis has changed and may not match the current visual representation
    }

    public void EndCall(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        CallManager.EndCallInManager(volunteerId, assignmentId);
    }

    public BO.Call GetCallDetails(int callId)
    {
        return CallManager.GetCallDetailsInManager(callId);
    }

    public int[] GetCallQuantitiesByStatus()
    {
        // get the grouped calls by status
        var groupedCalls = CallManager.returnCallInList()
                                       .GroupBy(call => call.Status)
                                       .ToDictionary(gr => gr.Key, gr => gr.Count());

        // ensure every status in the CallStatus enum is included, even those with no calls (0 count)
        return Enum.GetValues(typeof(CallStatus))
                   .Cast<CallStatus>()
                   .OrderBy(status => status)
                   .Select(status => groupedCalls.ContainsKey(status) ? groupedCalls[status] : 0)
                   .ToArray();
    }


    public IEnumerable<ClosedCallInList> GetClosedCallsHandledByVolunteer(int volunteerId, BO.CallType? filterBy, ClosedCallInListField? sortBy)
    {
        IEnumerable<ClosedCallInList> closedCallInLists = CallManager.listOfClosedCall(volunteerId);
        if (filterBy.HasValue)
            closedCallInLists = closedCallInLists.Where(call => call.CType == filterBy);
        
        closedCallInLists = sortBy switch
        {
            ClosedCallInListField.Id => closedCallInLists.OrderBy(call => call.Id),
            ClosedCallInListField.CType => closedCallInLists.OrderBy(call => call.CType),
            ClosedCallInListField.Opening => closedCallInLists.OrderBy(call => call.Opening),
            ClosedCallInListField.EnterTime => closedCallInLists.OrderBy(call => call.EnterTime),
            ClosedCallInListField.EndTime => closedCallInLists.OrderBy(call => call.EndTime),
            ClosedCallInListField.EType => closedCallInLists.OrderBy(call => call.EType),
            ClosedCallInListField.FullAddress => closedCallInLists.OrderBy(call => call.FullAddress),
            null => closedCallInLists.OrderBy(call => call.Id),  // if sortBy is null - sort by Id
            _ => throw new BlInvalidInputException("Error - invalid field to sort by.")
        };

        return closedCallInLists;
    }
    
    public IEnumerable<CallInList> GetFilteredAndSortedCalls(CallInListField? filterBy , object? filterValue, CallInListField? sortBy)
    {
        IEnumerable<CallInList> callsIL = Helpers.CallManager.returnCallInList();

        if (filterBy != null && filterValue == null)
            throw new BlInvalidInputException("Error - filtering by null value.");
        callsIL = filterBy switch
        {
            CallInListField.Id => callsIL.Where(c => c.Id == Convert.ToInt32(filterValue)).ToList(),
            CallInListField.CallId => callsIL.Where(c => c.CallId == Convert.ToInt32(filterValue)).ToList(),
            CallInListField.CType => callsIL.Where(c => c.CType == (BO.CallType)(int)filterValue).ToList(),
            CallInListField.Opening => callsIL.Where(c => c.Opening == (DateTime)filterValue).ToList(),
            CallInListField.TimeLeft => callsIL.Where(c => c.TimeLeft == (TimeSpan)filterValue).ToList(),
            CallInListField.LastVolunteer => callsIL.Where(c => c.LastVolunteer == (string)filterValue).ToList(),
            CallInListField.HandlingTime => callsIL.Where(c => c.HandlingTime == (TimeSpan)filterValue).ToList(),
            CallInListField.Status => callsIL.Where(c => c.Status == (BO.CallStatus)filterValue).ToList(),
            CallInListField.TotalAssignments => callsIL.Where(c => c.TotalAssignments == Convert.ToInt32(filterValue)).ToList(),
            null => callsIL,    // if filterBy is null - dont filter
            _ => throw new BlInvalidInputException("Error - invalid field to filter by.")
        };

        callsIL = sortBy switch
        {
            CallInListField.Id => callsIL.OrderBy(c => c.Id).ToList(),
            CallInListField.CallId => callsIL.OrderBy(c => c.CallId).ToList(),
            CallInListField.CType => callsIL.OrderBy(c => c.CType).ToList(),
            CallInListField.Opening => callsIL.OrderBy(c => c.Opening).ToList(),
            CallInListField.TimeLeft => callsIL.OrderBy(c => c.TimeLeft).ToList(),
            CallInListField.LastVolunteer => callsIL.OrderBy(c => c.LastVolunteer).ToList(),
            CallInListField.HandlingTime => callsIL.OrderBy(c => c.HandlingTime).ToList(),
            CallInListField.Status => callsIL.OrderBy(c => c.Status).ToList(),
            CallInListField.TotalAssignments => callsIL.OrderBy(c => c.TotalAssignments).ToList(),
            null => callsIL.OrderBy(c => c.Id).ToList(),    // if sortBy is null - sort by Id
            _ => throw new BlInvalidInputException("Error - invalid field to sort by.")
        };

        return callsIL;
    }


    public IEnumerable<CallInList> GetFilteredAndSortedCallsDoubleFiltering(BO.CallStatus? statusFilter, BO.CallType? callTypeFilter, CallInListField? sortBy)
    {
        IEnumerable<CallInList> callsIL = Helpers.CallManager.returnCallInList();


        if (callTypeFilter == null && statusFilter == null)
        { } // empty statement because we dont need to filter and we already have all the calls in the list
        else if (callTypeFilter == null && statusFilter != null)
            callsIL = callsIL.Where(c => c.Status == statusFilter);
        else if (statusFilter == null && callTypeFilter != null)
            callsIL = callsIL.Where(c => c.CType == callTypeFilter);
        else
            callsIL = callsIL.Where(c => c.Status == statusFilter && c.CType == callTypeFilter);

        callsIL = sortBy switch
        {
            CallInListField.Id => callsIL.OrderBy(c => c.Id).ToList(),
            CallInListField.CallId => callsIL.OrderBy(c => c.CallId).ToList(),
            CallInListField.CType => callsIL.OrderBy(c => c.CType).ToList(),
            CallInListField.Opening => callsIL.OrderBy(c => c.Opening).ToList(),
            CallInListField.TimeLeft => callsIL.OrderBy(c => c.TimeLeft).ToList(),
            CallInListField.LastVolunteer => callsIL.OrderBy(c => c.LastVolunteer).ToList(),
            CallInListField.HandlingTime => callsIL.OrderBy(c => c.HandlingTime).ToList(),
            CallInListField.Status => callsIL.OrderBy(c => c.Status).ToList(),
            CallInListField.TotalAssignments => callsIL.OrderBy(c => c.TotalAssignments).ToList(),
            null => callsIL.OrderBy(c => c.Id).ToList(),    // if sortBy is null - sort by Id
            _ => throw new BlInvalidInputException("Error - invalid field to sort by.")
        };

        return callsIL;
    }


    public IEnumerable<OpenCallInList> GetOpenCallsOfVolunteer(int volunteerId, BO.CallType? filterBy, BO.OpenCallInListField? sortBy)
    {
        return CallManager.GetOpenCallsOfVolunteerInManager(volunteerId, filterBy, sortBy);
    }


    public void UpdateCall(BO.Call cl)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        CallManager.UpdateCallInManager(cl);
    }


    // function to use in the convertor in the pl to check if a call can be deleted
    public bool IsDeletableCall(int callId)
    {
        return CallManager.canBeDeleted(callId);
    }



    public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver); //stage 5

    public void AddObserver(int id, Action observer) =>
        CallManager.Observers.AddObserver(id, observer); //stage 5

    public void RemoveObserver(Action listObserver) =>
        CallManager.Observers.RemoveListObserver(listObserver); //stage 5

    public void RemoveObserver(int id, Action observer) =>
        CallManager.Observers.RemoveObserver(id, observer); //stage 5
}
