using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Playroom;

public class CoinInserter : MonoBehaviour
{
    public void InsertCoinBTN()
    {
        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        {
            defaultStates = new() {
                {"score", 0},
                }
        }, OnLaunch);
    }

    private void OnLaunch()
    {
        Debug.Log("Game launched");
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;
#endif
        SceneManager.LoadScene("Game");
    }

    void OnDisconnect()
    {
        Debug.Log("Disconnection");
    }

}
