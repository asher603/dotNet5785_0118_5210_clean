using System.Runtime.CompilerServices;

namespace Dal;
internal static class Config
{
    internal const int startCallId = 0;
    private static int nextCallId = startCallId;
    internal static int NextCallId {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => nextCallId++; 
    }

    internal const int startAssingmentId = 0;
    private static int nextAssingmentId = startAssingmentId;
   
    internal static int NextAssingmentId {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => nextAssingmentId++; 
    }

    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;
    
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssingmentId = startAssingmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero;
    }
}


