using System;
using System.Collections.Generic;
using System.Diagnostics;
using AOT;
using Playroom;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using System.Collections;


public class GameManager : MonoBehaviour
{

    [Header("Player holders")]
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();

    private static Dictionary<string, GameObject> PlayerDict = new();


    [Header("Score and Score UI")]
    [SerializeField] private int score;
    [SerializeField] private Text scoreTextPlayer1;
    [SerializeField] private Text scoreTextPlayer2;

    private Text selectedScoreText;

    private static bool playerJoined;


    void Awake()
    {
        // StartCoroutine(StartMatchmakingCoroutine());
        Initialize();
    }

    private void Initialize()
    {
        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        {
            maxPlayersPerRoom = 2,

            defaultPlayerStates = new() {
                        {"score", 0},
                    },

        }, () =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
        });
    }

    private IEnumerator StartMatchmakingCoroutine()
    {
        Initialize();

        yield return new WaitUntil(() => PlayroomKit.GetPlayers().Count > 0);

        PlayroomKit.StartMatchmaking();
    }

    void Start()
    {
        PlayroomKit.RpcRegister("ShootBullet", HandleScoreUpdate, "You shot a bullet!");
        PlayroomKit.RpcRegister("Hello", Hello);
    }

    private void Hello(string arg1, string arg2)
    {
        print("helo");
    }

    void HandleScoreUpdate(string data, string caller)
    {
        var player = PlayroomKit.GetPlayer(caller);
        Debug.Log($"Caller: {caller}, Player Name: {player?.GetProfile().name}, Data: {data}");


        if (PlayerDict.TryGetValue(caller, out GameObject playerObj))
        {

            var playerController = playerObj.GetComponent<PlayerController>();
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

    private void Update()
    {
        if (playerJoined)
        {
            var myPlayer = PlayroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            playerGameObjects[index].GetComponent<PlayerController>().Move();

            players[index].SetState("pos", playerGameObjects[index].GetComponent<Transform>().position);

            ShootBullet(index);
            // SayHello();

            if (Input.GetKeyDown(KeyCode.R) && PlayroomKit.IsHost())
            {
                PlayroomKit.ResetStates(null, () =>
                {
                    var defscore = PlayroomKit.GetState<int>("score");
                    selectedScoreText.text = "Score: " + defscore.ToString();
                });

            }


            for (var i = 0; i < players.Count; i++)
            {

                if (players[i] != null)
                {
                    var pos = players[i].GetState<Vector3>("pos");
                    if (playerGameObjects[i] != null)
                        playerGameObjects[i].GetComponent<Transform>().position = pos;
                }


            }
        }

    }

    private void ShootBullet(int pleyerIndex)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 playerPosition = playerGameObjects[pleyerIndex].transform.position;
            playerGameObjects[pleyerIndex].GetComponent<PlayerController>().ShootBullet(
                playerPosition,
                50f);
            score += 50;
            PlayroomKit.RpcCall("ShootBullet", score, () =>
            {
                Debug.Log("Shooting bullet");
            });
        }
    }

    private void SayHello()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayroomKit.RpcCall("Hello", -1, () => {
                Debug.Log("saying helo");
            });
        }
    }

    public void AddPlayer(PlayroomKit.Player player)
    {
        GameObject playerObj = (GameObject)Instantiate(Resources.Load("Player"),
            new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0), Quaternion.identity);

        playerObj.GetComponent<SpriteRenderer>().color = player.GetProfile().color;

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);

        selectedScoreText = (players.Count == 1) ? scoreTextPlayer1 : scoreTextPlayer2;
        playerObj.GetComponent<PlayerController>().scoreText = selectedScoreText;

        playerJoined = true;
        player.OnQuit(RemovePlayer);
    }

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void RemovePlayer(string playerID)
    {
        if (PlayerDict.TryGetValue(playerID, out GameObject player))
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