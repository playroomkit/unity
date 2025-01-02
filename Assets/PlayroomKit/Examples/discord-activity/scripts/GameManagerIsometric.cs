using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManagerIsometric : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();

    private static readonly List<GameObject> playerGameObjects = new();

    private static readonly Dictionary<string, GameObject> PlayerDict = new();

    [SerializeField]
    private static bool playerJoined;

    [SerializeField]
    private string roomCode;

    [SerializeField]
    private GameObject playerPrefab;

    private PlayroomKit _playroomKit;

    private void Awake()
    {
        _playroomKit = new PlayroomKit();

    }

    private void Start()
    {

        _playroomKit.InsertCoin(new InitOptions
        {
            maxPlayersPerRoom = 3,
            matchmaking = false,
            discord = true,
            gameId = "ii4pV1wfceCjjLvRoo3O",
            roomCode = "roomCode",
        }, () =>
        {
            _playroomKit.OnPlayerJoin(AddPlayer);

            _playroomKit.RpcRegister("one", ((data, player) => { Debug.LogWarning("One Event Called"); }));

            _playroomKit.RpcRegister("two", ((data, player) => { Debug.LogWarning("two Event Called"); }));

            _playroomKit.RpcRegister("one",
                ((data, player) => { Debug.LogWarning("One Event Called With a diff callback"); }));
        }, () => { Debug.Log("OnDisconnect callback"); });
    }

    private void Update()
    {
        if (playerJoined)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space Down");
                _playroomKit.RpcCall("one", "69420", PlayroomKit.RpcMode.ALL);
                // _playroomKit.RpcCall("two", "2", PlayroomKit.RpcMode.ALL);
            }

            var myPlayer = _playroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            playerGameObjects[index].GetComponent<IsometricPlayerController>().LookAround();
            players[index].SetState("angle", playerGameObjects[index].GetComponent<Transform>().rotation);

            playerGameObjects[index].GetComponent<IsometricPlayerController>().Move();
            players[index].SetState("move", playerGameObjects[index].GetComponent<Transform>().position);

            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null && PlayerDict.TryGetValue(players[i].id, out GameObject playerObj))
                {
                    var pos = players[i].GetState<Vector3>("move");
                    var rotate = players[i].GetState<Quaternion>("angle");
                    var color = players[i].GetState<Color>("color");

                    if (playerGameObjects[i] != null)
                    {
                        playerGameObjects[i].GetComponent<Transform>().SetPositionAndRotation(pos, rotate);
                        playerGameObjects[i].GetComponent<Renderer>().material.color = color;
                    }
                }
            }
        }
    }

    private void AddPlayer(PlayroomKit.Player player)
    {
        Debug.LogFormat("{0} Is host?: {1}", player.GetProfile().name, _playroomKit.IsHost());

        var playerObj = Instantiate(playerPrefab,
            new Vector3(Random.Range(-5, 5), 2f, Random.Range(-5, 5)), Quaternion.identity);

        player.SetState("color", player.GetProfile().color);
        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);

        for (var i = 0; i < players.Count; i++) Debug.Log($"player at index {i} is {players[i].GetProfile().name}");

        playerJoined = true;
        player.OnQuit(RemovePlayer);
    }

    private static void RemovePlayer(string playerID)
    {
        if (PlayerDict.TryGetValue(playerID, out var player))
        {
            PlayerDict.Remove(playerID);
            playerGameObjects.Remove(player);
            Destroy(player);

            foreach (var (key, value) in PlayerDict) Debug.Log($"player {key} is still in the room");

            Debug.Log(playerID + " has left the room!");
        }
        else
        {
            Debug.LogWarning("player not in dict");
        }
    }
}