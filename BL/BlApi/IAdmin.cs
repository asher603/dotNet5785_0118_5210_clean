namespace BlApi;

public interface IAdmin
{
    /// <summary>
    /// Retrieves the current system date and time.
    /// 
    /// This method returns the current value of the system clock as a `DateTime` object, representing the current date and time.
    /// </summary>
    /// <returns>
    /// A `DateTime` object representing the current system date and time.
    /// </returns>
    public DateTime GetClock();

    /// <summary>
    /// Advances the system clock by the specified time unit.
    /// 
    /// The method accepts an enumeration value representing a time unit (minute, hour, day, month, year) and advances the system clock accordingly.
    /// A new time value is created by adding the specified time unit to the current system time, and the updated time is passed to the static helper method `AdminManager.UpdateClock`.
    /// For example, if the time unit is 'minute' (`BO.TimeUnit.MINUTE`), the new time will be calculated as `AdminManager.Now.AddMinutes(1)` and the method will be called as: 
    /// `AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1))`.
    /// </summary>
    /// <param name="timeUnit">
    /// The time unit by which to advance the clock. This can be one of the following values:
    /// - `minute`
    /// - `hour`
    /// - `day`
    /// - `month`
    /// - `year`
    /// </param>
    public void AdvanceClock(BO.TimeUnit timeUnit);

    /// <summary>
    /// Retrieves the configured risk time range.
    /// 
    /// This method does not accept any parameters and returns the configured "risk range" as a `TimeSpan` value.
    /// The risk range represents a predefined time period used for risk-related calculations or decision-making.
    /// </summary>
    /// <returns>
    /// A `TimeSpan` representing the configured risk time range.
    /// </returns>
    public TimeSpan GetRiskRange();

    /// <summary>
    /// Sets the configured risk time range.
    /// 
    /// This method accepts a `TimeSpan` value representing the risk time range and updates the configuration with the provided value.
    /// The updated risk range will be used for subsequent risk-related calculations or decision-making.
    /// </summary>
    /// <param name="riskRange">
    /// A `TimeSpan` representing the new risk time range to be set.
    /// </param>
    public void SetRiskRange(TimeSpan riskRange);

    /// <summary>
    /// Resets the configuration and data entities.
    /// 
    /// This method does not accept any parameters and does not return a value.
    /// It resets all configuration data to their default initial values and clears the data of all entities (empties all data lists).
    /// This is typically used for resetting the system to its initial state.
    /// </summary>
    public void ResetDB();

    /// <summary>
    /// Initializes the database.
    /// 
    /// This method does not accept any parameters and does not return any value.
    /// It performs a full initialization of the database by first resetting it, then populating it with initial data for all entities.
    /// The data for each entity is added according to the database initialization requirements, filling all data lists with default values.
    /// </summary>
    public void InitializeDB();

    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7

    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);

}
