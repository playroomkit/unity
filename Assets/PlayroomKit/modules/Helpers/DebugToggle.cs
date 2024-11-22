using UnityEngine;

public class DebugToggle : MonoBehaviour
{
    [SerializeField] private bool enableLogs = false;

    private void Start()
    {
        DebugLogger.enableLogs = enableLogs;
    }

    private void ToggleLogs(bool isEnabled)
    {
        DebugLogger.enableLogs = isEnabled;
    }
}

