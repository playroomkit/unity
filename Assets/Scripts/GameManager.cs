using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private PlayroomKit kit;

    void Start()
    {
        kit = new();
        kit.InsertCoin(new InitOptions()
        {
            gameId = "[my game id]",
            maxPlayersPerRoom = 8,
            discord = true
        }, () =>
        {
            kit.RpcRegister("A", A);
            kit.RpcRegister("B", B);
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            kit.RpcCall("A", "", PlayroomKit.RpcMode.ALL);
        else if (Input.GetMouseButtonDown(1))
            kit.RpcCall("B", "", PlayroomKit.RpcMode.ALL);
    }

    private void A(string data, string senderID)
    {
        Debug.Log("A");
    }

    private void B(string data, string senderID)
    {
        Debug.Log("B");
    }
}