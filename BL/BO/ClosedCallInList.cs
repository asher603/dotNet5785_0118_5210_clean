using Helpers;
using DO;

namespace BO;
public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CType { get; init; }
    public string FullAddress { get; init; }
    public DateTime Opening { get; init; }
    public DateTime EnterTime { get; init; }
    public DateTime? EndTime { get; init; }
    public EndingType? EType { get; init; }
    public override string ToString() => this.ToStringProperty();
}

