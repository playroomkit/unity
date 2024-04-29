using UnityEngine;
using Playroom;
using AOT;
using System;
using TMPro;


public class Lobby : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI currentRoomCode;
    [SerializeField] private string roomID;

    private string gameURL;
    private Action unsubOnQuit;

    public void Initialize()
    {
        PlayroomKit.InsertCoin(
        new PlayroomKit.InitOptions() { roomCode = roomID },
        () => { PlayroomKit.OnPlayerJoin(AddPlayer); }
        );
    }

    public void LeaveRoom()
    {
        PlayroomKit.Player player = PlayroomKit.MyPlayer();

        player.OnQuitWrapperCallback();
        currentRoomCode.text = "Room Code: " + PlayroomKit.GetRoomCode();

        unsubOnQuit();
    }

    private void AddPlayer(PlayroomKit.Player player)
    {
        currentRoomCode.text = "Room Code: " + PlayroomKit.GetRoomCode();
        Debug.Log($"{player.GetProfile().name} joining room no {PlayroomKit.GetRoomCode()}");
        unsubOnQuit = player.OnQuit(RemovePlayer);
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private void RemovePlayer(string id)
    {
        var p = PlayroomKit.GetPlayer(id).GetProfile().name;
        Debug.Log($"{p} leaving room no {PlayroomKit.GetRoomCode()}");
    }

    public void ReadString(string newRoomCode)
    {
        roomID = newRoomCode;
        Debug.Log("room code set: " + roomID);
    }

}
