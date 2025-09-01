using Helpers;
namespace BO;

public class Volunteer
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? FullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Role VRole { get; set; }
    public bool Active { get; set; }
    public double? Distance { get; set; }
    public DistanceType VDisType { get; init; } = DistanceType.Air;
    public int TotalCallsHandled { get; init; }
    public int TotalCallsCancelled { get; init; }
    public int TotalCallsExpired { get; init; }
    public BO.CallInProgress? CallInProgress { get; set; }
    public override string ToString() => this.ToStringProperty();
}

