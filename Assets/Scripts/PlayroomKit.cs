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
    private static extern void SetStateInternal(string key, string value);

    public static void SetState<T>(string key, T value)
    {
        if (IsRunningInBrowser())
        {
            // Convert the value to a string representation (JSON) and call the non-generic SetState method
            string valueStr = JsonUtility.ToJson(value);
            SetStateInternal(key, valueStr);
        }
        else
        {
            Debug.LogWarning("SetState Mock");
        }
    }


    [DllImport("__Internal")]
    private static extern string GetStateInternal(string key);

    public static T GetState<T>(string key)
    {
        if (IsRunningInBrowser())
        {
            // Call the non-generic GetState method and convert the result to the desired type
            string valueStr = GetStateInternal(key).ToString();
            return JsonUtility.FromJson<T>(valueStr);
        }
        else
        {
            Debug.LogWarning("GetState Mock");
            return default;
        }
    }

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
