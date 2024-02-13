using System;
using System.Collections.Generic;
using System.Diagnostics;
using AOT;
using Playroom;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;


public class GameManager : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();

    private static Dictionary<string, GameObject> PlayerDict = new();
    Dictionary<string, float> moveData = new();

    [SerializeField] private int score;
    [SerializeField] private Text scoreText1;
    [SerializeField] private Text scoreText2;

    [SerializeField] private static bool playerJoined;

    bool isMoving;

    private void Awake()
    {
        // PlayroomKit.OnPlayerJoin(AddPlayer);

        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() {
                        {"score", -500},
                    },

        }, () =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
        });

    }

    void Start()
    {
        PlayroomKit.RpcRegister("ShootBullet", HandleScoreUpdate, "You shot a bullet!");
    }

    void HandleScoreUpdate(string data, string caller)
    {
        var player = PlayroomKit.GetPlayer(caller);
        Debug.Log($"Caller: {caller}, Player Name: {player?.GetProfile().name}, Data: {data}");

        // Find the player's GameObject in the dictionary
        if (PlayerDict.TryGetValue(caller, out GameObject playerObj))
        {
            // Get the PlayerController component
            var playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Update the score text
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
            players[index].SetState("posX", playerGameObjects[index].GetComponent<Transform>().position.x);
            players[index].SetState("posY", playerGameObjects[index].GetComponent<Transform>().position.y);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 playerPosition = playerGameObjects[index].transform.position;
                playerGameObjects[index].GetComponent<PlayerController>().ShootBullet(playerPosition, 50f);

                score += 50;

                // if (PlayerDict.TryGetValue(myPlayer.id, out GameObject playerObj))
                // {
                //     var playerController = playerObj.GetComponent<PlayerController>();
                //     if (playerController != null)
                //     {
                //         playerController.scoreText.text = $"Score: {score}";
                //     }
                // }

                PlayroomKit.RpcCall("ShootBullet", score, () =>
                {
                    Debug.Log("shooting bullet!");
                });
            }

            if (Input.GetKeyDown(KeyCode.R) && PlayroomKit.IsHost())
            {
                PlayroomKit.ResetStates(null, () =>
                {
                    var defscore = PlayroomKit.GetState<int>("score");
                    // scoreText.text = "Score: " + defscore.ToString();
                });

            }
        }

        for (var i = 0; i < players.Count; i++)
        {

            if (players[i] != null)
            {
                var posX = players[i].GetState<float>("posX");
                var posY = players[i].GetState<float>("posY");
                Vector3 newPos = new Vector3(posX, posY, 0);

                if (playerGameObjects != null)
                    playerGameObjects[i].GetComponent<Transform>().position = newPos;
            }

            // if (PlayroomKit.IsHost())
            // {
            //     if (playerGameObjects[i].GetComponent<Transform>().position.x >= 0f)
            //     {
            //         score += 10;
            //         scoreText.text = "Score: " + score.ToString();
            //         PlayroomKit.SetState("score", score);
            //     }
            // }
            // else
            // {
            //     scoreText.text = "Score: " + PlayroomKit.GetState<int>("score").ToString();
            // }
        }

    }

    void rpcRegisterCallback(string data, string sender)
    {
        Debug.Log("sender: " + sender);
        var player = PlayroomKit.GetPlayer(sender);
        Debug.Log("Name of sender: " + player.GetProfile().name);
    }


    public void AddPlayer(PlayroomKit.Player player)
    {
        GameObject playerObj = (GameObject)Instantiate(Resources.Load("Player"),
            new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0), Quaternion.identity);

        playerObj.GetComponent<SpriteRenderer>().color = player.GetProfile().color;
        Debug.Log(player.GetProfile().name + " Joined the game!" + "id: " + player.id);

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);

        Text scoreText = (players.Count == 1) ? scoreText1 : scoreText2;
        playerObj.GetComponent<PlayerController>().scoreText = scoreText;


        playerJoined = true;

        player.OnQuit(RemovePlayer);
    }



    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void RemovePlayer(string playerID)
    {
        if (PlayerDict.TryGetValue(playerID, out GameObject player))
        {
            Destroy(player);
        }
        else
        {
            Debug.LogWarning("player not in dict");
        }

    }
}