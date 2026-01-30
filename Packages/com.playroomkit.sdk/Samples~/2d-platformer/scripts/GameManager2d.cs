using Playroom;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


public class GameManager2d : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    /// <summary>
    /// player scores and UI to display score of the game.
    /// </summary>
    [Header("Score and UI")]
    [SerializeField]
    private int score = 0;
    [SerializeField]
    private TextMeshProUGUI playerCount;
    [SerializeField]
    private TextMeshProUGUI scoreTemplate;
    [SerializeField]
    private Transform scoreList;

    private bool playersJoined;

    /// <summary>
    /// List of players and their gameObjects.
    /// </summary>
    private readonly Dictionary<string, PlayerEntity> players = new();

    private PlayroomKit _playroomKit;


    void Awake()
    {
        _playroomKit = new();
    }

    /// <summary>
    /// Initialize PlayroomKit, starts multiplayer.
    /// </summary>
    private void Initialize()
    {
        _playroomKit.InsertCoin(
            new InitOptions
            {
                maxPlayersPerRoom = 4,
                defaultPlayerStates = new()
                {
                    { "score", 0 }
                }
            },
            onLaunchCallBack: () =>
            {
                _playroomKit.OnPlayerJoin(AddPlayer);
                Debug.Log($"[Unity Log] isHost: {_playroomKit.IsHost()}");
            },
            onDisconnectCallback: () =>
            {
                Debug.LogWarning("OnDisconnect Callback Called");
            }
        );

        scoreTemplate.gameObject.SetActive(false);
    }


    /// <summary>
    /// Register the RPC method to update the score.
    /// </summary>
    void Start()
    {
        Initialize();

        _playroomKit.RpcRegister("ShootBullet", HandleScoreUpdate, "Bullet Shot!");

    }

    /// <summary>
    /// Update the Score UI of the player and sync.
    /// </summary>
    void HandleScoreUpdate(string data, string caller)
    {
        if (players.TryGetValue(caller, out PlayerEntity playerEntity))
        {
            Debug.Log($"Caller: {caller}, Player Name: {playerEntity.Player?.GetProfile().name}, Data: {data}");

            if (!int.TryParse(data, out int parsedScore))
            {
                Debug.LogError($"Failed to parse score data: {data}");
                return;
            }
            if (!playerEntity.IsMyPlayer)
            {
                playerEntity.Controller.ShootBullet();
            }

            playerEntity.UpdateScoreText(parsedScore);
        }
        else
        {
            Debug.LogError($"No PlayerEntity found for caller: {caller}");
        }
    }

    /// <summary>
    /// Update the player position and sync.
    /// </summary>
    private void Update()
    {
        if (playersJoined && _playroomKit.MyPlayer() != null)
        {
            var myPlayerId = _playroomKit.MyPlayer().id;

            if (players.TryGetValue(myPlayerId, out PlayerEntity myPlayerEntity))
            {
                myPlayerEntity.Controller.Move();
                myPlayerEntity.Controller.Jump();
                myPlayerEntity.Player.SetState("pos", myPlayerEntity.GameObject.transform.position);
                myPlayerEntity.Player.SetState("facing", myPlayerEntity.Controller.FacingDir);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ShootBullet(myPlayerEntity);
                }

                UpdateRemotePlayers();
            }
            else
            {
                Debug.LogError($"No PlayerEntity found for my player ID: {myPlayerId}");
            }
        }
    }

    private void UpdateRemotePlayers()
    {
        foreach (var playerEntry in players)
        {
            var playerEntity = playerEntry.Value;

            if (playerEntity.IsMyPlayer) continue;

            if (playerEntity.Player != null && playerEntity.GameObject != null)
            {
                var pos = playerEntity.Player.GetState<Vector3>("pos");
                var facing = playerEntity.Player.GetState<float>("facing");
                playerEntity.Controller.ApplyRemoteState(pos, facing);
            }
            else
            {
                Debug.LogError($"PlayerEntity not found for player: {playerEntry.Key}");
            }

        }
    }


    /// <summary>
    /// Shoot bullet and update the score.
    /// </summary>
    private void ShootBullet(PlayerEntity playerEntity)
    {
        playerEntity.Controller.ShootBullet();
        score += 10;

        playerEntity.Player.SetState("score", score);

        _playroomKit.RpcCall("ShootBullet", score, PlayroomKit.RpcMode.ALL,
            () => { Debug.Log("Shooting bullet"); });

    }

    /// <summary>
    /// Adds the "player" to the game scene.
    /// </summary>
    public void AddPlayer(PlayroomKit.Player player)
    {
        if (players.ContainsKey(player.id))
        {
            Debug.LogWarning($"Player {player.id} already exists in the game.");
            return;
        }

        Vector3 spawnPos;
        if (_playroomKit.MyPlayer() != null && player.id == _playroomKit.MyPlayer().id)
        {
            spawnPos = new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0);
            player.SetState("facing", 1f);
            player.SetState("pos", spawnPos);
            SetupPlayerEntity(player, spawnPos, true);
        }
        else
        {
            player.WaitForState("pos", response =>
            {
                var pos = player.GetState<Vector3>("pos");
                spawnPos = pos;
                SetupPlayerEntity(player, spawnPos, false);
                Debug.Log($"Player {player.id} joined.");
            });
        }
    }

    private void SetupPlayerEntity(PlayroomKit.Player player, Vector3 spawnPos, bool isMyPlayer)
    {
        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        var scoreObj = Instantiate(scoreTemplate, scoreList);

        scoreObj.text = $"{player.GetProfile().name}: {player.GetState<int>("score")}";

        var playerEntity = new PlayerEntity(player, playerObj, scoreObj, isMyPlayer);
        players.Add(player.id, playerEntity);

        player.OnQuit(RemovePlayer);

        UpdatePlayerCount();
    }

    /// <summary>
    /// Remove player from the game, called when the player leaves / closes the game.
    /// </summary>
    private void RemovePlayer(string playerID)
    {
        if (players.TryGetValue(playerID, out PlayerEntity playerEntity))
        {
            players.Remove(playerID);
            playerEntity.DestroyObjects();
            UpdatePlayerCount();

            Debug.Log($"Player {playerID} removed successfully.");
        }
        else
        {
            Debug.LogWarning($"Player {playerID} is not in dictionary.");
        }
    }

    private void UpdatePlayerCount()
    {
        playerCount.text = $"Players: {players.Count}";
        playersJoined = players.Count > 0;
    }

    private sealed class PlayerEntity
    {
        public PlayroomKit.Player Player { get; }
        public GameObject GameObject { get; }
        public PlayerController2d Controller { get; }
        public SpriteRenderer SpriteRenderer { get; }
        public bool IsMyPlayer { get; }

        public PlayerEntity(PlayroomKit.Player player, GameObject gameObject, TextMeshProUGUI scoreObject, bool isMyPlayer)
        {
            Player = player;
            GameObject = gameObject;
            IsMyPlayer = isMyPlayer;
            scoreObject.gameObject.SetActive(true);
            Controller = gameObject.GetComponent<PlayerController2d>();
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            var color = player.GetProfile().color;

            SpriteRenderer.color = color;
            scoreObject.color = color;

            Controller.scoreText = scoreObject;

        }

        public void UpdateScoreText(int score)
        {
            if (Controller != null && Controller.scoreText != null && Player != null && Player.GetProfile() != null)
            {
                Controller.scoreText.text = $"{Player.GetProfile().name}: {score}";
            }
        }

        public void DestroyObjects()
        {
            Destroy(Controller.scoreText.gameObject);
            Destroy(GameObject);
        }
    }

}

