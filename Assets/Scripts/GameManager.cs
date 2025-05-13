using System;
using System.Collections.Generic;
using Playroom;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit playroomKit;

    public TextMeshProUGUI text;

    bool coinInserted = false;

    string skuId = "1371921246031319121";

    private void Awake()
    {
        playroomKit = new PlayroomKit();
    }

    private void Start()
    {
        playroomKit.InsertCoin(new InitOptions()
        {
            gameId = "FmOBeUfQO2AOLNIrJNSJ",
            maxPlayersPerRoom = 2,
            discord = true,
        }, OnLaunchCallBack);
    }

    private void OnLaunchCallBack()
    {
        playroomKit.OnPlayerJoin(CreatePlayer);
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
            Debug.Log("Token: " + playroomKit.GetPlayroomToken());
            text.text = playroomKit.GetPlayroomToken();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            playroomKit.OpenDiscordInviteDialog(() =>
            {
                text.text = "Discord invite dialog opened!";
            });
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            // After InsertCoin has fully invoked
            playroomKit.StartDiscordPurchase(skuId, (response) =>
            {
                Debug.Log($"Entitlement: {response}");
            });
        }
    }
}