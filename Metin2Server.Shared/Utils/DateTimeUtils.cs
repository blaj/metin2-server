namespace Metin2Server.Shared.Utils;

public static class DateTimeUtils
{
    private static DateTime? bootTime = null;

    public static uint GetUnixTime()
    {
        var currentTime = DateTime.UtcNow;
        var elapsedSeconds = (long) (currentTime - DateTime.UnixEpoch).TotalSeconds - GetBootTime();
        
        return (uint)(elapsedSeconds * 1000 + currentTime.Millisecond);
    }
    
    public static long GetBootTime()
    {
        if (bootTime == null)
        {
            bootTime = DateTime.UtcNow;
        }

        // Return the number of seconds since bootTime
        return (long)(bootTime - DateTime.UnixEpoch)?.TotalSeconds!;
    }

    public static uint GetGlobalTime()
    {
        return (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}