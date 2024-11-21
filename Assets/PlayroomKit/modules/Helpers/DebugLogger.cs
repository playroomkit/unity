using UnityEngine;

public static class DebugLogger
{
#if UNITY_EDITOR || DEBUG
    [SerializeField] private static UnityEngine.UI.Toggle logToggle;

    public static void Log(string message)
    {
        if (logToggle.isOn)
        {
            Debug.Log(message);
        }
    }

    public static void LogWarning(string message)
    {
        if (logToggle.isOn)
        {
            Debug.LogWarning(message);
        }
    }

    public static void LogError(string message)
    {
        if (logToggle.isOn)
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