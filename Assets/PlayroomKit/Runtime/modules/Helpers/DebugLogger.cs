using UnityEngine;

public static class DebugLogger
{
    public static bool enableLogs = true; // Toggle to enable/disable logs
#if UNITY_EDITOR || DEBUG
    

    public static void Log(string message)
    {
        if (enableLogs)
        {
            Debug.Log(message);
        }
    }

    public static void LogWarning(string message)
    {
        if (enableLogs)
        {
            Debug.LogWarning(message);
        }
    }

    public static void LogError(string message)
    {
        if (enableLogs)
        {
            Debug.LogError(message);
        }
    }
#else
    public static void Log(string message) { }
    public static void LogWarning(string message) { }
    public static void LogError(string message) { }
#endif
}