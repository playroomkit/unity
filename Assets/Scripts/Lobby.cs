using UnityEngine;
using Playroom;
using AOT;
using System;
using TMPro;
using System.Collections.Generic;


public class Lobby : MonoBehaviour
{
    [SerializeField] private List<string> playerNames = new();
    [SerializeField] private TextMeshProUGUI currentPlayerName;

    [SerializeField] private List<Texture2D> Avatars = new();
    

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        currentPlayerName.text = playerNames[0];
    }


    private void Initialize()
    {
        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() {
                        {"score", 0},
                    },

        }, () =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
        });
    }

    private void AddPlayer(PlayroomKit.Player player)
    {
        string playerName = player.GetProfile().name;


        playerNames.Add(playerName);
    }

    
}
