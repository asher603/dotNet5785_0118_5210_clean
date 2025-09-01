namespace DO;
/// <summary>
/// Call Entity represents a call with all its props
/// </summary>
/// <param name="Id"> unique ID to identify the call (created automatically - provide 0 as an argument)</param>
/// <param name="Description">Description of the call</param>
/// <param name="FullAddress">The full adress of the call</param>
/// <param name="Latitude">The latitude of the loction</param>
/// <param name="Longitude">The longitude of the loction</param>
/// <param name="Opening">The time when the call was open</param>
/// <param name="MaxTime">The time the call should end</param>

public record Call
(
    int Id,
    string Description,
    string FullAddress,
    DateTime Opening,
    double Latitude,
    double Longitude,
    CallType CType = CallType.None,
    DateTime? MaxTime = null
    
)
{
    public Call() : this(0, "", "", DateTime.Now, 0.0, 0.0) { }
}

