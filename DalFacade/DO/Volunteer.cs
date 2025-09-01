namespace DO;
/// <summary>
/// Volunteer Entity represents a volunteer with all its props
/// </summary>
/// <param name="Id"> unique ID to identify the call </param>
/// <param name="FullName">The full name of the volunteer</param>
/// <param name="Email">The email of the volunteer</param>
/// <param name="FullAddress">The full adress of the volunteer</param>
/// <param name="Latitude">The latitude of the loction</param>
/// <param name="Longitude">The longitude of the loction</param>
/// <param name="Active">The volunteer is active</param>
/// <param name="Distance">The distance the volunteer seted for getting calls</param>
public record Volunteer
(
    int Id,
    string FullName,
    string PhoneNumber,
    string Email,
    string Password,
    string? FullAddress = null,
    bool Active = false,
    double? Distance = null,
    Role VRole = Role.Manager,
    DistanceType VDisType = DistanceType.Air,
    double? Latitude = null,
    double? Longitude = null
    
)
{
    public Volunteer() : this(0, "", "","", "", "") { }
}
