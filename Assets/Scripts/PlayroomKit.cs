using UnityEngine;
using System.Runtime.InteropServices;

public class PlayroomKit : MonoBehaviour
{

    [DllImport("__Internal")]
    public static extern void Jump(string str);

    [DllImport("__Internal")]
    public static extern void LoadPlayroom();

    [DllImport("__Internal")]
    public static extern void InsertCoin();


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
