using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using UnityEngine;
using Random = UnityEngine.Random;

public class IsometricManager : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();

    private static readonly Dictionary<string, GameObject> PlayerDict = new();


    [SerializeField] private static bool playerJoined;

    [SerializeField] private GameObject playerPrefab;


    private void Awake()
    {
        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions
        {
            maxPlayersPerRoom = 3,
            matchmaking = false,
            gameId = "iQwdxMdv3fCASVrghYgf",
            discord = true
        }, () => { PlayroomKit.OnPlayerJoin(AddPlayer); });
    }

    private void Update()
    {
        if (playerJoined)
        {
            var myPlayer = PlayroomKit.MyPlayer();
            var i = players.IndexOf(myPlayer);

            playerGameObjects[i].GetComponent<IsometricPlayerController>().LookAround();
            players[i].SetState("angle", playerGameObjects[i].GetComponent<Transform>().rotation);

            playerGameObjects[i].GetComponent<IsometricPlayerController>().Move();
            players[i].SetState("move", playerGameObjects[i].GetComponent<Transform>().position);
        }

        for (var i = 0; i < players.Count; i++)
            if (players[i] != null)
            {
                var pos = players[i].GetState<Vector3>("move");
                var rotate = players[i].GetState<Quaternion>("angle");

                if (playerGameObjects[i] != null)
                    playerGameObjects[i].GetComponent<Transform>().SetPositionAndRotation(pos, rotate);
            }
    }


    public void AddPlayer(PlayroomKit.Player player)
    {
        var playerObj = Instantiate(playerPrefab,
            new Vector3(Random.Range(-5, 5), 2f, Random.Range(-5, 5)), Quaternion.identity);

        playerObj.GetComponent<Renderer>().material.color = player.GetProfile().color;

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);


        for (var i = 0; i < players.Count; i++) Debug.Log($"player at index {i} is {players[i].GetProfile().name}");


        playerJoined = true;
        player.OnQuit(RemovePlayer);
    }


    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void RemovePlayer(string playerID)
    {
        if (PlayerDict.TryGetValue(playerID, out var player))
        {
            PlayerDict.Remove(playerID);
            playerGameObjects.Remove(player);
            Destroy(player);
        }
        else
        {
            Debug.LogWarning("player not in dict");
        }
    }
}