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
            // same event multiple RPCs
            _kit.RpcRegister("A", A);
            _kit.RpcRegister("A", A2);

            // RPC Call From Another RPC
            _kit.RpcRegister("host", (data, sender) =>
            {
                    Debug.Log("Host RPC CALLED");
                    _kit.RpcCall("client", 1, PlayroomKit.RpcMode.ALL);
            }
            );
            _kit.RpcRegister("client", (data, sender) =>
            {
                Debug.Log("client rpc called");
            });
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _kit.RpcCall("A", 1, PlayroomKit.RpcMode.HOST);

        if (Input.GetMouseButtonDown(1))
            _kit.RpcCall("host", 1, PlayroomKit.RpcMode.HOST);
    }

    private void A(string data, string sender)
    {
        Debug.Log($"[Unity] A called");
    }

    private void A2(string data, string sender)
    {
        Debug.LogWarning("[Unity] A2 data: " + data.ToString());
    }
}