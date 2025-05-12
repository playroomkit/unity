using System;
using System.Collections.Generic;
using Playroom;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit _kit;

    public TextMeshProUGUI text;

    bool coinInserted = false;

    private void Awake()
    {
        _kit = new PlayroomKit();
    }

    private void Start()
    {
        _kit.InsertCoin(new InitOptions()
        {
            gameId = "cW0r8UJ1aXnZ8v5TPYmv",
            maxPlayersPerRoom = 2,
            discord = true,
        }, OnLaunchCallBack);
    }

    private void OnLaunchCallBack()
    {
        _kit.OnPlayerJoin(CreatePlayer);
        coinInserted = true;
    }

    private void CreatePlayer(PlayroomKit.Player player)
    {
        Debug.Log($"{player.id} joined the room!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Token: " + _kit.GetPlayroomToken());
            text.text = _kit.GetPlayroomToken();
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            _kit.OpenDiscordInviteDialog(()=>
            {
                text.text = "Discord invite dialog opened!";
            });
        }

        // if (Input.GetKeyDown(KeyCode.P) && coinInserted)
        // {
        //     _kit.StartDiscordPurchase("1234", (response) => {
        //     });
        // }
    }

   
}