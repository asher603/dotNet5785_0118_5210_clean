using Helpers;
namespace BO;
public class CallInProgress
{
    public int AsmId { get; init; }
    public int CallId { get; init; }
    public CallType CType { get; init; }
    public string? Description { get; init; }
    public string FullAddress { get; init; }
    public DateTime Opening { get; init; }
    public DateTime? MaxTime { get; init; }
    public DateTime EnterTime { get; init; }
    public double Distance { get; init; }
    public CallStatus Status { get; set; }

    public override string ToString() => this.ToStringProperty();
}
