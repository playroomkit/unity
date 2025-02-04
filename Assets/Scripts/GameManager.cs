using System;
using Playroom;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit _kit;

    private void Awake()
    {
        _kit = new();
    }

    private void Start()
    {
        _kit.InsertCoin(new InitOptions()
        {
            turnBased = true,
            maxPlayersPerRoom = 2,
        }, OnLaunchCallBack);
    }

    private void OnLaunchCallBack()
    {
        _kit.OnPlayerJoin(CreatePlayer);
        
        // _kit.get
    }

    private void CreatePlayer(PlayroomKit.Player player)
    {
        Debug.Log($"{player.id} joined the room!");
    }

    private void Update()
    {
    }
}