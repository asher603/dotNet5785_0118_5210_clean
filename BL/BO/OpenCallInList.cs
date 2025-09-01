using Helpers;
namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }
    public CallType CType { get; init; }
    public string? Description { get; init; }
    public string FullAddress { get; init; }
    public DateTime Opening { get; init; }
    public DateTime? MaxTime { get; init; }
    public double Distance { get; init; }

    public override string ToString() => this.ToStringProperty();
}
