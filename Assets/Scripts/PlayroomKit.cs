using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class PlayroomKit : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void Jump(string str);

    [DllImport("__Internal")]
    public static extern void LoadPlayroom();

    [DllImport("__Internal")]
    public static extern void InsertCoin();

    [DllImport("__Internal")]
    public static extern void SetState(string key, int value);

    [DllImport("__Internal")]
    public static extern int GetState(string key);

    [DllImport("__Internal")]
    public static extern bool IsHost();

    // it checks if the game is running in the browser or in the editor
    public static bool IsRunningInBrowser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}
