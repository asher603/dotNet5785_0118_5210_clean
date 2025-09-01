using Helpers;
namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? FullName { get; init; }
    public DateTime EnterTime { get; init; }
    public DateTime? EndTime { get; init; }
    public EndingType? EType { get; init; }

    public override string ToString() => this.ToStringProperty();
}
