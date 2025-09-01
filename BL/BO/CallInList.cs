using Helpers;
namespace BO;

public class CallInList
{
    public int? Id { get; init; }
    public int CallId { get; init; }
    public CallType CType { get; init; }
    public DateTime Opening { get; init; }
    public TimeSpan? TimeLeft { get; init; }
    public string? LastVolunteer { get; init; }
    public TimeSpan? HandlingTime { get; init; }
    public CallStatus Status { get; init; }
    public int TotalAssignments { get; init; }

    public override string ToString() => this.ToStringProperty();
}
