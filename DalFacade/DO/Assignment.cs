namespace DO;
/// <summary>
/// Assignment Entity represents a call with all its props
/// </summary>
/// <param name="Id"> Personal unique ID of the assignment (created automatically - provide 0 as an argument) </param>
/// <param name="CallId"> number of call the Volunteer handles </param>
/// <param name="VolunteerId"> id of the Volunteer who handle  that call </param>
/// <param name="EnterTime"> the time the current call entered for handling </param>
/// <param name="EType"> the type of ending </param>
/// <param name="EndTime"> the time the handling of current call ended </param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime EnterTime,
    EndingType? EType = null,
    DateTime? EndTime = null
)
{
    public Assignment() : this(0, 0, 0 , DateTime.Now) { }
}