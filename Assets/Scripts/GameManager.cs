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
            PlayroomKit.RPC.RpcRegister2("A", A);
            PlayroomKit.RPC.RpcRegister2("B", B);
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            kit.RpcCall("A", 1, PlayroomKit.RpcMode.ALL);

        if (Input.GetMouseButtonDown(1))
            kit.RpcCall("B", 2, PlayroomKit.RpcMode.ALL);
        
        
    }

    private void A(string data)
    {
        Debug.Log($"[Unity] A data: {data}");
    }

    private void B(string data)
    {
        Debug.Log($"[Unity] B data: {data}");
    }
}