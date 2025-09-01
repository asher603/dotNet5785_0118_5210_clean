namespace BlImplementation;

using BlApi;
using BO;

using Helpers;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.Role GetVolunteerRoleAndValidatePasswordForRegistration(int volunteerId, string password)
    {
        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex)
            doVolunteer = _dal.Volunteer.Read(v => v.Id == volunteerId);
        if (doVolunteer == null)
        {
            throw new BlDoesNotExistException($"volunteer with Id {volunteerId} does not exists");
        }

        // first we check if the password is null, because the getHashPassword function cant handle null value as parameter
        if (password == null || doVolunteer.Password != VolunteerManager.getHashPassword(password))
        {
            throw new BlPasswordInCorrect("The password is incorrect");
        }
        return (BO.Role)doVolunteer.VRole;

    }


    public BO.Role GetVolunteerRole(int volunteerId)
    {
        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex)
            doVolunteer = _dal.Volunteer.Read(v => v.Id == volunteerId);
        if (doVolunteer == null)
        {
            throw new BlDoesNotExistException($"volunteer with Id {volunteerId} does not exists");
        }
        return (BO.Role)doVolunteer.VRole;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(VolunteerFilterOptions? fieldToFilterBy, object? filterFieldValue, VolunteerInListFields? fieldToSortBy)
    {
        if (fieldToFilterBy != null && filterFieldValue == null)
            throw new BlInvalidInputException("Error - filtering by null value.");
        IEnumerable<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex)
        {
            volunteers = fieldToFilterBy switch
            {
                VolunteerFilterOptions.Active => _dal.Volunteer.ReadAll(v => v.Active == (bool)filterFieldValue!),
                VolunteerFilterOptions.CType => _dal.Volunteer.ReadAll(v => VolunteerManager.GetVolunteerCurrentCallType(v.Id) == (BO.CallType)filterFieldValue!),
                null => _dal.Volunteer.ReadAll(),
                _ => throw new BlInvalidInputException("Error - invalid field to filter by.")
            };
        }

        // creating BO list from DO list so we can return it
        var volunteersList = volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            Active = v.Active,
            TotalCallsHandled = VolunteerManager.CalculateTotalCallsHandled(v.Id),
            TotalCallsCancelled = VolunteerManager.CalculateTotalCallsCancelled(v.Id),
            TotalCallsExpired = VolunteerManager.CalculateTotalCallsExpired(v.Id),
            CallInProgressId = VolunteerManager.GetCurrentCallId(v.Id),
            CType = VolunteerManager.GetVolunteerCurrentCallType(v.Id)
        });

        volunteersList = fieldToSortBy switch
        {
            VolunteerInListFields.Id => volunteersList.OrderBy(v => v.Id),
            VolunteerInListFields.FullName => volunteersList.OrderBy(v => v.FullName),
            VolunteerInListFields.Active => volunteersList.OrderBy(v => v.Active),
            VolunteerInListFields.TotalCallsHandled => volunteersList.OrderBy(v => v.TotalCallsHandled),
            VolunteerInListFields.TotalCallsCancelled => volunteersList.OrderBy(v => v.TotalCallsCancelled),
            VolunteerInListFields.TotalCallsExpired => volunteersList.OrderBy(v => v.TotalCallsExpired),
            VolunteerInListFields.CallInProgressId => volunteersList.OrderBy(v => v.CallInProgressId),
            VolunteerInListFields.CType => volunteersList.OrderBy(v => v.CType),
            null => volunteersList.OrderBy(v => v.Id),  // if fieldToSortBy is null - sort by volunteer Id
            _ => throw new BlInvalidInputException("Error - invalid field to sort by.")
        };

        return volunteersList;  // *** maybe need to return volunteersList.ToList() ***
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersListDoubleFiltering(bool? activationStatusFilter, BO.CallType? callTypeFilter, VolunteerInListFields? fieldToSortBy)
    {
        IEnumerable<DO.Volunteer> volunteers;
        if (callTypeFilter == null && activationStatusFilter == null)
        {
            lock (AdminManager.BlMutex)
                volunteers = _dal.Volunteer.ReadAll(); 
        }
        else if (callTypeFilter == null && activationStatusFilter != null)
        {
            lock (AdminManager.BlMutex)
                volunteers = _dal.Volunteer.ReadAll(v => v.Active == activationStatusFilter);
        }
        else if (activationStatusFilter == null && callTypeFilter != null)
        {
            lock (AdminManager.BlMutex)
                volunteers = _dal.Volunteer.ReadAll(v => VolunteerManager.GetVolunteerCurrentCallType(v.Id) == callTypeFilter);
        }
        else
        {
            lock (AdminManager.BlMutex)
                volunteers = _dal.Volunteer.ReadAll(v => v.Active == activationStatusFilter && VolunteerManager.GetVolunteerCurrentCallType(v.Id) == callTypeFilter);
        }

        // creating BO list from DO list so we can return it
        var volunteersList = volunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            Active = v.Active,
            TotalCallsHandled = VolunteerManager.CalculateTotalCallsHandled(v.Id),
            TotalCallsCancelled = VolunteerManager.CalculateTotalCallsCancelled(v.Id),
            TotalCallsExpired = VolunteerManager.CalculateTotalCallsExpired(v.Id),
            CallInProgressId = VolunteerManager.GetCurrentCallId(v.Id),
            CType = VolunteerManager.GetVolunteerCurrentCallType(v.Id)
        });

        volunteersList = fieldToSortBy switch
        {
            VolunteerInListFields.Id => volunteersList.OrderBy(v => v.Id),
            VolunteerInListFields.FullName => volunteersList.OrderBy(v => v.FullName),
            VolunteerInListFields.Active => volunteersList.OrderBy(v => v.Active),
            VolunteerInListFields.TotalCallsHandled => volunteersList.OrderBy(v => v.TotalCallsHandled),
            VolunteerInListFields.TotalCallsCancelled => volunteersList.OrderBy(v => v.TotalCallsCancelled),
            VolunteerInListFields.TotalCallsExpired => volunteersList.OrderBy(v => v.TotalCallsExpired),
            VolunteerInListFields.CallInProgressId => volunteersList.OrderBy(v => v.CallInProgressId),
            VolunteerInListFields.CType => volunteersList.OrderBy(v => v.CType),
            null => volunteersList.OrderBy(v => v.Id),  // if fieldToSortBy is null - sort by volunteer Id
            _ => throw new BlInvalidInputException("Error - invalid field to sort by.")
        };

        return volunteersList;
    }

    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        DO.Volunteer? doVolunteer;
        // Retrieve volunteer data from the data layer
        lock (AdminManager.BlMutex)
            doVolunteer = _dal.Volunteer.Read(v => v.Id == volunteerId);

        if (doVolunteer == null)
        {
            throw new BlDoesNotExistException($"volunteer with Id {volunteerId} does not exists");
        }
        // Create a new Volunteer object from the retrieved data
        var volunteer = new BO.Volunteer
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            Password = doVolunteer.Password,
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            VRole = (BO.Role)doVolunteer.VRole,
            Active = doVolunteer.Active,
            Distance = doVolunteer.Distance,
            VDisType = (BO.DistanceType)doVolunteer.VDisType,
            TotalCallsHandled = VolunteerManager.CalculateTotalCallsHandled(doVolunteer.Id),
            TotalCallsCancelled = VolunteerManager.CalculateTotalCallsCancelled(doVolunteer.Id),
            TotalCallsExpired = VolunteerManager.CalculateTotalCallsExpired(doVolunteer.Id),
            CallInProgress = VolunteerManager.GetCurrentCall(doVolunteer.Id)
        };

        // Return the created Volunteer object
        return volunteer;
    }

    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer updatedVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        VolunteerManager.UpdateVolunteerDetailsInManager(requesterId, updatedVolunteer);
    }

    public void DeleteVolunteer(int volunteerId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Assignment? doAssignmentOfVolunteer;
        // set doAssinment to be a collection of all assignments of with calls that the volunteer is currently handling or alredy handled in the past
        lock (AdminManager.BlMutex)
            doAssignmentOfVolunteer = _dal.Assignment.Read(asm => asm.VolunteerId == volunteerId);

        // if doAssignmentOfVolunteer is not null, it means the volunteer is currently handling or handled a call and therefore can not be deleted
        if (doAssignmentOfVolunteer != null)
            throw new BlDoesNotExistException($"volunteer with Id {volunteerId} is currently handling or already handled a call and therefore can not be deleted");

        try
        {
            _dal.Volunteer.Delete(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5  
        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException("Failed to delete volunteer", ex);
        }

        VolunteerManager.Observers.NotifyItemUpdated(volunteerId);  // notify observers that the logical state of the BO.Volunteer with that id has changed and may not match the current visual representation (this volunteer is now deleted)
        VolunteerManager.Observers.NotifyListUpdated();  // notify observers that the logical state of the list of BO.Volunteers has changed and may not match the current visual representation
    }

    public void AddVolunteer(BO.Volunteer volunteerToAdd)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();

            // Validate all fields except for the address those which requires a call to the net so we will do it separately in async func to save time
            VolunteerManager.ValidateVolunteerFieldsExceptAddress(volunteerToAdd);

            DO.Volunteer doVolunteerToAdd = new DO.Volunteer
            {
                Id = volunteerToAdd.Id,
                FullName = volunteerToAdd.FullName,
                PhoneNumber = volunteerToAdd.PhoneNumber,
                Email = volunteerToAdd.Email,
                Password = VolunteerManager.getHashPassword(volunteerToAdd.Password),
                FullAddress = string.Empty,
                Active = volunteerToAdd.Active,
                Distance = volunteerToAdd.Distance,
                VRole = (DO.Role)volunteerToAdd.VRole,
                VDisType = (DO.DistanceType)volunteerToAdd.VDisType,
                Latitude = 0.0,
                Longitude = 0.0
            };

            lock (AdminManager.BlMutex)
                _dal.Volunteer.Create(doVolunteerToAdd);

            VolunteerManager.Observers.NotifyListUpdated();

            // after we done with adding the volunteer fields except for his address and coordinates, and also notified the observers, we can now update the address and coordinates

            // creating a new object of DO.volunteer wth the new address (unlike doVolunteer which had the old address since we didnt know yes if the new address is valid when sending him in the update function)
            DO.Volunteer doVolunteerWithNewAddress = doVolunteerToAdd with { FullAddress = volunteerToAdd.FullAddress };

            //compute the coordinates asynchronously without waiting for the results
            _ = VolunteerManager.validateAddressAndUpdateAddressAndCoordinatesForVolunteerAsync(doVolunteerWithNewAddress);
        }
        catch (Exception ex)
        {
            throw new BlAlreadyExistsException("Failed to add volunteer", ex);
        }

    }

    public string GeneratePassword() => VolunteerManager.GeneratePassword();

    public int RatePassword(string password) => VolunteerManager.RatePassword(password);


    public void AddObserver(Action listObserver) =>
        VolunteerManager.Observers.AddListObserver(listObserver); //stage 5 

    public void AddObserver(int id, Action observer) =>
        VolunteerManager.Observers.AddObserver(id, observer); //stage 5

    public void RemoveObserver(Action listObserver) =>
        VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5

    public void RemoveObserver(int id, Action observer) =>
        VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5

}
