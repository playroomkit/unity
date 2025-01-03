using Playroom;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit _kit;

    private void Start()
    {
        _kit = new();
        
        _kit.InsertCoin(new InitOptions()
        {
            gameId = "[my game id]",
            maxPlayersPerRoom = 8,
            discord = true
        }, () =>
        {
            _kit.RpcRegister("A", A);
            _kit.RpcRegister("B", B);
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _kit.RpcCall("A", 1, PlayroomKit.RpcMode.HOST);
        
        if (Input.GetMouseButtonDown(1))
            Debug.LogWarning(_kit.IsHost());
    }

    private void A(string data, string sender)
    {
        Debug.Log($"[Unity] A called only on HOST {data} and {sender}");
        
        _kit.TransferHost(sender);

        _kit.RpcCall("B", 2, PlayroomKit.RpcMode.OTHERS);
    }

    private void B(string data, string sender)
    {
        Debug.Log($"[Unity] B called on ALL data: {data} and {sender}");
        
        
    }
}