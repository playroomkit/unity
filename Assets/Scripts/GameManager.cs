using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static bool playerJoined;

    /// <summary>
    ///     List of players and their gameObjects.
    /// </summary>
    [SerializeField] private List<PlayroomKit.Player> players = new();

    private static readonly List<GameObject> playerGameObjects = new();
    private static readonly Dictionary<string, GameObject> PlayerDict = new();

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private TextMeshProUGUI playerIDText;
    [SerializeField] private TextMeshProUGUI score;


    [SerializeField] private string playerID;

    private void Awake()
    {
        PlayroomKit.InsertCoin(new()
            {
                skipLobby = true,
            },
            () => PlayroomKit.OnPlayerJoin(AddPlayer));
    }


    /// <summary>
    ///     Update the player position and sync.
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            PlayroomKit.SetState("global", 500);
            Debug.Log(PlayroomKit.GetRoomCode());
            PlayroomKit.IsHost();
        }

        if (Input.GetKey(KeyCode.L))
        {
            score.text = $"IDK: {PlayroomKit.GetState<int>("global").ToString()}";
        }

        if (!playerJoined) return;

        var player = PlayroomKit.Me();
        var index = players.IndexOf(player);


        // Move and sync the local player's position
        playerGameObjects[index].GetComponent<Player>().Move();
        player.SetState("pos", playerGameObjects[index].transform.position);


        // Update other players' positions
        for (var i = 0; i < players.Count; i++)
        {
            // if (players[i] == myPlayer) continue;

            if (players[i] != null)
            {
                var posX = PlayroomKit.MyPlayer().GetState<Vector3>("pos");
                // var posY = PlayroomKit.GetPlayer(players[i]).GetState<float>("posY");

                if (playerGameObjects != null)
                {
                    // Vector3 pos = new(posX, posY, 0);
                    playerGameObjects[i].GetComponent<Transform>().position = posX;
                }
            }
        }
    }


    /// <summary>
    ///     Adds the "player" to the game scene.
    /// </summary>
    public void AddPlayer(PlayroomKit.Player player)
    {
        playerIDText.text += $"{player.id} joined the game!";

        playerID = player.id;

        var spawnPos = new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0);
        var playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        Debug.Log($"<color=#ADD8E6>Player ID: {player.id}</color>");

        Debug.Log($"<color=#ADD8E6>Player Name: {player.GetProfile().color}</color>");
        Debug.Log($"<color={player.GetProfile().playerProfileColor.hexString}>Player ID: {player.GetProfile().name}</color>");

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);

        playerJoined = true;
        player.OnQuit(RemovePlayer);
    }

    /// <summary>
    ///     Remove player from the game, called when the player leaves / closes the game.
    /// </summary>
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
            Debug.LogWarning("Player is not in dictionary");
        }
    }
}