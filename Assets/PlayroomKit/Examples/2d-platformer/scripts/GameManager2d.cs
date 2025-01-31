using System;
using System.Collections.Generic;
using Playroom;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using TMPro;


public class GameManager2d : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private string roomCode;

    /// <summary>
    /// player scores and UI to display score of the game.
    /// </summary>
    [Header("Score and UI")]
    [SerializeField]
    private int score = 0;
    [SerializeField]
    private TextMeshProUGUI scoreTextPlayer1;
    [SerializeField]
    private TextMeshProUGUI scoreTextPlayer2;

    private TextMeshProUGUI selectedScoreText;

    private static bool playerJoined;

    /// <summary>
    /// List of players and their gameObjects.
    /// </summary>
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();
    private static Dictionary<string, GameObject> PlayerDict = new();

    private PlayroomKit _playroomKit;
    [SerializeField]
    private string newData;


    void Awake()
    {
        _playroomKit = new();
    }

    /// <summary>
    /// Initialize PlayroomKit, starts multiplayer.
    /// </summary>
    private void Initialize()
    {
        _playroomKit.InsertCoin(new InitOptions()
        {
            // roomCode = roomCode,
            persistentMode = true,
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new()
            {
                { "score", 0 },
            },
        }, () =>
        {
            _playroomKit.OnPlayerJoin(AddPlayer);
            print($"[Unity Log] isHost: {_playroomKit.IsHost()}");
        });
    }

    /// <summary>
    /// Register the RPC method to update the score.
    /// </summary>
    void Start()
    {
        Initialize();

        _playroomKit.RpcRegister("ShootBullet", HandleScoreUpdate, "You shot a bullet!");
        _playroomKit.WaitForState("test", (s) => { Debug.LogWarning($"After waiting for test: {s}"); });
    }

    /// <summary>
    /// Update the Score UI of the player and sync.
    /// </summary>
    void HandleScoreUpdate(string data, string caller)
    {
        var player = _playroomKit.GetPlayer(caller);
        Debug.Log($"Caller: {caller}, Player Name: {player?.GetProfile().name}, Data: {data}");

        if (PlayerDict.TryGetValue(caller, out GameObject playerObj))
        {
            var playerController = playerObj.GetComponent<PlayerController2d>();
            if (playerController != null)
            {
                playerController.scoreText.text = $"Score: {data}";
            }
            else
            {
                Debug.LogError($"PlayerController not found on GameObject for caller: {caller}");
            }
        }
        else
        {
            Debug.LogError($"No GameObject found for caller: {caller}");
        }
    }

    /// <summary>
    /// Update the player position and sync.
    /// </summary>
    private void Update()
    {
        if (playerJoined)
        {
            var myPlayer = _playroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            if (Input.GetKeyDown(KeyCode.L)) _playroomKit.SetState("test", "yes");

            playerGameObjects[index].GetComponent<PlayerController2d>().Move();
            playerGameObjects[index].GetComponent<PlayerController2d>().Jump();


            if (Input.GetKeyDown(KeyCode.N))
            {
                _playroomKit.SetPersistentData("TEST", new Vector3(1, 6, 4));
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _playroomKit.GetPersistentData("TEST", data =>
                {
                    Debug.Log($"Persistence data: {data}");
                });
            }


            players[index].SetState("pos", playerGameObjects[index].transform.position);
            ShootBullet(index);

            for (var i = 0; i < players.Count; i++)
            {
                if (players[i] != null && PlayerDict.TryGetValue(players[i].id, out GameObject playerObj))
                {
                    // Debug.Log("Getting state of: " + players[i].id);
                    var pos = players[i].GetState<Vector3>("pos");


                    var color = players[i].GetState<Color>("color");
                    if (playerGameObjects != null)
                    {
                        playerGameObjects[i].GetComponent<Transform>().position = pos;
                        playerGameObjects[i].GetComponent<SpriteRenderer>().color = color;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Shoot bullet and update the score.
    /// </summary>
    private void ShootBullet(int playerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 playerPosition = playerGameObjects[playerIndex].transform.position;

            score = playerGameObjects[playerIndex].GetComponent<PlayerController2d>()
                .ShootBullet(playerPosition, 50f, score);

            _playroomKit.RpcCall("ShootBullet", score, PlayroomKit.RpcMode.ALL,
                () => { Debug.Log("Shooting bullet"); });
        }
    }

    /// <summary>
    /// Adds the "player" to the game scene.
    /// </summary>
    public void AddPlayer(PlayroomKit.Player player)
    {
        var spawnPos = new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0);
        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        player.SetState("color", player.GetProfile().color);

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);

        selectedScoreText = (players.Count == 1) ? scoreTextPlayer1 : scoreTextPlayer2;
        playerObj.GetComponent<PlayerController2d>().scoreText = selectedScoreText;

        playerJoined = true;
        player.OnQuit(RemovePlayer);
    }

    /// <summary>
    /// Remove player from the game, called when the player leaves / closes the game.
    /// </summary>
    private static void RemovePlayer(string playerID)
    {
        if (PlayerDict.TryGetValue(playerID, out GameObject player))
        {
            PlayerDict.Remove(playerID);
            players.Remove(players.Find(p => p.id == playerID));
            playerGameObjects.Remove(player);
            Destroy(player);
        }
        else
        {
            Debug.LogWarning("Player is not in dictionary");
        }
    }
}

