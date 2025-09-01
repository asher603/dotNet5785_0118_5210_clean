namespace DO;


/// <param name="Role">The role of the volunteer</param>
public enum Role { Manager, Volunteer };

/// <param name="DistanceType">How you calculate the distance</param>
public enum DistanceType { Air, Walking, Driving };

/// <param name="CallType">The type of call</param>

public enum CallType { TireChange, JumpStart, FluidRefill, LightFix, LostKey, None }

/// <param name="EndingType"> enum containing all types of ending for the call handling </param>
public enum EndingType { Solved, SelfCanceld, CanceledByManager, Expired };

