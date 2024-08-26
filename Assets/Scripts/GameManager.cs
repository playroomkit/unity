using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using TMPro;
using UBB;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static bool playerJoined;

    /// <summary>
    ///     List of players and their gameObjects.
    /// </summary>
    [SerializeField] private List<string> players = new();

    private static readonly List<GameObject> playerGameObjects = new();
    private static readonly Dictionary<string, GameObject> PlayerDict = new();

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private TextMeshProUGUI playerIDText;
    [SerializeField] private TextMeshProUGUI score;


    [SerializeField] private string playerID;

    private void Awake()
    {
        PlayroomKit.OnPlayerJoin(AddPlayer);
    }


    /// <summary>
    ///     Update the player position and sync.
    /// </summary>
    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            PlayroomKit.SetState("global", 500);
        }

        if (Input.GetKey(KeyCode.L))
        {
            score.text = $"IDK: {PlayroomKit.GetState<int>("global").ToString()}";
        }

        if (playerJoined)
        {
            var myPlayer = playerID;
            var index = players.IndexOf(myPlayer);

            var player = PlayroomKit.GetPlayer(myPlayer);

            // Move and sync the local player's position
            playerGameObjects[index].GetComponent<Player>().Move();
            player.SetState("posX", playerGameObjects[index].transform.position.x);
            player.SetState("posY", playerGameObjects[index].transform.position.y);

            // Update other players' positions
            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] == myPlayer) continue;

                if (players[i] != null)
                {
                    var otherPlayer = PlayroomKit.GetPlayer(players[i]);
                    var posX = otherPlayer.GetState<float>("posX");
                    var posY = otherPlayer.GetState<float>("posY");


                    if (playerGameObjects != null)
                    {
                        Vector3 pos = new(posX, posY, 0);
                        playerGameObjects[i].GetComponent<Transform>().position = pos;
                    }
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

        PlayerDict.Add(player.id, playerObj);
        players.Add(player.id);
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