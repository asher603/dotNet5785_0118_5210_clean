using Helpers;
namespace BO;
public class VolunteerInList
{
    public int Id { get; init; }
    public string FullName { get; init; }
    public bool Active { get; init; }
    public int TotalCallsHandled { get; init; }
    public int TotalCallsCancelled { get; init; }
    public int TotalCallsExpired { get; init; }
    public int? CallInProgressId { get; init; }
    public CallType CType { get; init; }
    public override string ToString() => this.ToStringProperty();
}
