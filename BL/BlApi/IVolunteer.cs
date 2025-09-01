using BO;

namespace BlApi;

/// <summary>
/// Interface for managing volunteer-related operations.
/// </summary>
public interface IVolunteer : IObservable 
{
    /// <summary>
    /// For registeration, retrieves volunteeer role and checks if the password he tried to register with is correct.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to register.</param>
    /// <param name="password">The password that the volunteer tried to register with, if the password is incorrect it will throw an exception.</param>
    /// <returns>The role of the registered volunteer.</returns>
    /// <exception cref="BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    public BO.Role GetVolunteerRoleAndValidatePasswordForRegistration(int volunteerId, string password);

    /// <summary>
    /// Retrieves a list of volunteers sorted and filtered based on parameters.
    /// </summary>
    /// <param name="fieldToFilterBy"></param>
    /// <param name="filterFieldValue"></param>
    /// <param name="fieldToSortBy"></param>
    /// <returns>A sorted and filtered list of volunteers.</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(VolunteerFilterOptions? fieldToFilterBy, object? filterFieldValue, VolunteerInListFields? fieldToSortBy);


    /// <summary>
    /// Retrieves a list of volunteers sorted and filtered based on parameters.
    /// </summary>
    /// <param name="activationStatusFilter"></param>
    /// <param name="callTypeFilter"></param>
    /// <param name="fieldToSortBy"></param>
    /// <returns>A sorted and filtered list of volunteers.</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersListDoubleFiltering(bool? activationStatusFilter, BO.CallType? callTypeFilter, VolunteerInListFields? fieldToSortBy);

    /// <summary>
    /// Retrieves detailed information about a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to retrieve.</param>
    /// <returns>A <see cref="BO.Volunteer"/> object with detailed information.</returns>
    /// <exception cref="BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>


    public BO.Volunteer GetVolunteerDetails(int volunteerId);

    /// <summary>
    /// Updates the details of a volunteer.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the update.</param>
    /// <param name="updatedVolunteer">The updated volunteer details.</param>
    /// <exception cref="BlCanNotUpdateException">Thrown if the requester lacks permissions or if the update fails.</exception>
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer updatedVolunteer);

    /// <summary>
    /// Deletes a volunteer by their ID.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to delete.</param>
    /// <exception cref="BlDeletionImpossibleException">Thrown if the volunteer has related assignments.</exception>
    /// <exception cref="BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    public void DeleteVolunteer(int volunteerId);

    /// <summary>
    /// Adds a new volunteer.
    /// </summary>
    /// <param name="toAdd">The <see cref="BO.Volunteer"/> object containing the volunteer's details.</param>
    /// <exception cref="BlInvalidInputException">Thrown if the input is invalid.</exception>
    /// <exception cref="BlAlreadyExistsException">Thrown if the volunteer already exists.</exception>
    public void AddVolunteer(BO.Volunteer toAdd);

    /// <summary>
    /// The function creates new Password (for the Pl)
    /// </summary>
    /// <returns>the new password</returns>
    public string GeneratePassword();

    /// <summary>
    /// The function rates a password 0 - weak, 1 - meduim, 2 - strong (used to change the color of password foreground)
    /// </summary>
    public int RatePassword(string password);
}

