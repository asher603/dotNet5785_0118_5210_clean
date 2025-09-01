using BO;

namespace BlApi;

public interface ICall: IObservable
{
    /// <summary>
    /// Retrieves the quantities of calls, grouped by their status.
    /// This method is used on the system's login screen to display the number of calls in different statuses.
    /// </summary>
    /// <returns>
    /// An array of integers where each element corresponds to the quantity of calls for a specific status.
    /// The index of the array represents different statuses (e.g., 0: New, 1: In Progress, 2: Closed, etc.).
    /// </returns>
    public int[] GetCallQuantitiesByStatus();

    /// <summary>
    /// Filters and sorts a list of calls based on the specified field and value.
    /// </summary>
    /// <param name="filterBy">The field by which the list will be filtered (e.g., Id, CType, FullAddress, etc.)</param>
    /// <param name="filterValue">The value to filter the calls by (can be of any type, and may be null)</param>
    /// <param name="sortBy">The field by which the list will be sorted (e.g., Id, CType, FullAddress, etc.)</param>
    /// <returns>A filtered and sorted collection of CallInList entities</returns>
    public IEnumerable<CallInList> GetFilteredAndSortedCalls(CallInListField? filterBy, object filterValue, CallInListField? sortBy);

    /// <summary>
    /// Retrieves the details of a specific call by its unique identifier (callId).
    /// The method queries the data layer to fetch detailed information about the call, including any associated assignments (if available).
    /// It then constructs a logical entity object of type "Call" (BO.Call), which includes a list of logical entities of type "CallAssignInList" (List<BO.CallAssignInList>).
    /// This method is typically used in the "Single Call Management" screen.
    /// </summary>
    /// <param name="callId">
    /// The unique identifier of the call whose details are to be retrieved.
    /// </param>
    /// <returns>
    /// A "Call" entity (BO.Call) that includes detailed information about the call and its associated assignments, if any.
    /// </returns>


    public IEnumerable<CallInList> GetFilteredAndSortedCallsDoubleFiltering(BO.CallStatus? statusFilter, BO.CallType? callTypeFilter, CallInListField? sortBy);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>

    public BO.Call GetCallDetails(int callId);

    /// <summary>
    /// Updates the details of an existing call with the provided information.
    /// The method accepts a "Call" entity (BO.Call) that is fully populated with values. 
    /// It then validates the data, ensuring that:
    /// - All values are correctly formatted.
    /// - The logical consistency of the values is verified (e.g., the max time is greater than the opening time).
    /// - The address is valid and corresponds to a real-world location with valid latitude and longitude coordinates.
    /// After validating, the method updates the latitude and longitude in the BO.Call entity based on the provided address.
    /// The method then creates a corresponding "Call" data entity (DO.Call) and attempts to update the call in the data layer.
    /// This method does not return any value.
    /// </summary>
    /// <param name="cl">
    /// The "Call" entity (BO.Call) containing all the updated values for the call.
    /// The values in this object should be fully populated before calling this method.
    /// </param>
    public void UpdateCall(BO.Call cl);

    /// <summary>
    /// Deletes a call identified by its unique call ID, if possible.
    /// The call can only be deleted if it is in "Open" status and has never been assigned to a volunteer.
    /// If these conditions are not met, the call remains in the system for historical purposes.
    /// An exception is thrown if the deletion is not allowed.
    /// If eligible, the call is deleted from the data layer using the provided `callId`.
    /// </summary>
    /// <param name="callId">
    /// The unique identifier of the call to be deleted.
    /// </param>
    public void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call to the system.
    /// 
    /// The method accepts a fully populated `BO.Call` object and validates its values, ensuring:
    /// - The data format is correct.
    /// - The logical consistency of the values (e.g., similar to the `UpdateCall` method).
    /// 
    /// A new `DO.Call` data object is created from the `BO.Call` details, and the method attempts to add the new call to the data layer using the `Create` method.
    /// This method does not return any value.
    /// </summary>
    /// <param name="cl">
    /// The "Call" entity (BO.Call) containing the details of the new call to be added.
    /// The object should be fully populated with the required values.
    /// </param>
    public void AddCall(BO.Call cl);

    /// <summary>
    /// Retrieves a list of closed calls handled by a specific volunteer.
    /// 
    /// The method accepts the `volunteerId` to filter closed calls that were handled by the given volunteer.
    /// Optionally, the list can be filtered by the call type (`filterBy`) and sorted by a specific field (`sortBy`).
    /// 
    /// - If the `filterBy` parameter is not provided (null), the list will include all closed calls for the volunteer.
    /// - If the `filterBy` is provided, the list will be filtered by the specified call type.
    /// - If the `sortBy` parameter is not provided (null), the list will be sorted by call ID.
    /// - If the `sortBy` is provided, the list will be sorted by the specified field.
    /// 
    /// The method returns a sorted and optionally filtered collection of closed calls (BO.ClosedCallInList).
    /// </summary>
    /// <param name="volunteerId">
    /// The unique identifier of the volunteer whose closed calls are to be retrieved.
    /// </param>
    /// <param name="filterBy">
    /// An optional enum value of the call type (`BO.CallType`) to filter the list by. 
    /// If null, no filtering
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsHandledByVolunteer(int volunteerId, BO.CallType? filterBy, ClosedCallInListField? sortBy);

    /// <summary>
    /// Retrieves a list of open calls available for selection by a specific volunteer.
    /// 
    /// The method accepts a `volunteerId` to return a list of open calls that the volunteer can choose from, along with their distance from the volunteer's current location.
    /// 
    /// - The list can be filtered by `CallType` (if provided) and sorted by a specific field (`sortBy`).
    /// - Only calls with the status "Open" or "Open with Risk" will be included.
    /// - If `filterBy` is `null`, all calls are returned without filtering by call type.
    /// - If `sortBy` is `null`, the list will be sorted by call ID.
    /// - If `sortBy` is provided, the list will be sorted according to the specified field.
    /// 
    /// The returned list will include the distance of each call from the volunteer.
    /// </summary>
    /// <param name="volunteerId">
    /// The unique identifier of the volunteer for whom the open calls are being retrieved.
    /// </param>
    /// <param name="filterBy">
    /// An optional enum value of the call type (`BO.CallType`) to filter the list by. 
    /// If null, no filtering is applied based on call type.
    /// </param>
    /// <param name="sortBy">
    /// An optional enum value of the field (`BO.OpenCallInListField`) by which to sort the list.
    /// If null, the list will be sorted by call ID.
    /// </param>
    /// <returns>
    /// A sorted and optionally filtered list of open call entities (
    public IEnumerable<BO.OpenCallInList> GetOpenCallsOfVolunteer(int volunteerId, BO.CallType? filterBy, BO.OpenCallInListField? sortBy);

    /// <summary>
    /// Marks the end of treatment for a specific call by a volunteer.
    /// 
    /// The method accepts a `volunteerId` and an `assignmentId` to update the status of the assignment to "Completed" (i.e., treatment ended).
    /// It performs the following checks:
    /// - Verifies that the volunteer is the one assigned to the call.
    /// - Verifies that the assignment is still open (not completed, canceled, or expired).
    /// - Ensures that the actual end time of the assignment is not already set (i.e., the call is still active).
    /// If the request is invalid, an appropriate exception is thrown.
    /// 
    /// Upon successful validation, the method updates the assignment record in the data layer, setting:
    /// - The treatment status to "Completed".
    /// - The actual end time to the current system time.
    /// </summary>
    /// <param name="volunteerId">
    /// The unique identifier of the volunteer who is reporting the end of treatment.
    /// </param>
    /// <param name="assignmentId">
    /// The unique identifier of the assignment for which the volunteer is reporting the end of treatment.
    /// </param>
    public void EndCall(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels an assignment for a specific call.
    /// 
    /// The method accepts a `requestorId` and an `assignmentId` to cancel an assignment that is currently being handled by a volunteer.
    /// It performs the following checks:
    /// - Verifies that the requestor has permission to cancel the assignment:
    ///   - The requestor must be either the volunteer assigned to the call or an administrator.
    /// - Checks that the assignment is still open (not completed or expired).
    /// - Ensures that the treatment has not yet been completed (the actual end time is still null).
    /// If the request is invalid, an appropriate exception is thrown.
    /// 
    /// Upon successful validation, the method updates the assignment record in the data layer:
    /// - Sets the treatment status to "Canceled".
    /// - Sets the actual end time to the current system time.
    /// - If the `requestorId` matches the volunteer's ID, the cancellation type is set to "Self-Cancellation".
    /// - If the `requestorId` does not match the volunteer's ID, the cancellation type is set to "Admin Cancellation".
    /// </summary>
    /// <param name="requestorId">
    /// The unique identifier of the requestor initiating the cancellation. 
    /// This can either be the volunteer's ID or an administrator's ID.
    /// </param>
    /// <param name="assignmentId">
    /// The unique identifier of the assignment to be canceled.
    /// </param>
    public void CancelAssignment(int requestorId, int assignmentId);

    /// <summary>
    /// Assigns a volunteer to a call for treatment.
    /// 
    /// The method accepts a `volunteerId` and a `callId` to assign the specified volunteer to the selected call for treatment.
    /// It performs the following checks to validate the request:
    /// - The call has not yet been treated or assigned to any volunteer.
    /// - The call is not expired and is still open for assignment.
    /// - The call is not already under treatment by another volunteer.
    /// If the request is invalid, an appropriate exception is thrown.
    /// 
    /// Upon successful validation, the method creates a new assignment record in the data layer:
    /// - Associates the volunteer with the selected call.
    /// - Sets the "Time of Assignment" to the current system time.
    /// - Leaves the "Actual End Time" and "Treatment Type" as null for now.
    /// </summary>
    /// <param name="volunteerId">
    /// The unique identifier of the volunteer who is accepting the call for treatment.
    /// </param>
    /// <param name="callId">
    /// The unique identifier of the call the volunteer is choosing to handle.
    /// </param>
    public void ChooseCall(int volunteerId, int callId);


    public bool IsDeletableCall(int callId);


}
