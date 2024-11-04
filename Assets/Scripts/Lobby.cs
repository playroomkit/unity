using System;
using UnityEngine;
using Playroom;
using UnityEngine.SceneManagement;


public class Lobby : MonoBehaviour
{
    private PlayroomKit _playroomKit = new PlayroomKit();
    public void Initialize()
    {
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            skipLobby = true,
            defaultPlayerStates = new()
            {
                { "score", 0 },
            },
        }, onLaunchCallBack);
    }

    private void onLaunchCallBack()
    {
        SceneManager.LoadScene("topdown");
    }
}