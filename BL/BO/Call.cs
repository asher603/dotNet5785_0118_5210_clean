using Helpers;
namespace BO;

public class Call
{
    public int Id { get; init; }
    public CallType CType { get; init; }
    public string? Description { get; init; }
    public string FullAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Opening { get; init; }
    public DateTime? MaxTime { get; set; }
    public CallStatus Status { get; set; }

    public List<BO.CallAssignInList>? CallAssignInList { get; set; }
    
    public override string ToString() => this.ToStringProperty();
}
