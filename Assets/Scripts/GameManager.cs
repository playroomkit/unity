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
    [SerializeField] private Text scoreText;

    [SerializeField] private static bool playerJoined;

    Dictionary<string, float> states;


    private void Awake()
    {

        // PlayroomKit.OnPlayerJoin(AddPlayer);

        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() {
                        {"score", -500},
                    },
            // skipLobby = true,
        }, () =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
            PlayroomKit.SetState("score", score);

        });

        states = new() {
                        {"health", 86.4f},
                        {"speed", 10.34f},
                    };

    }


    private void Update()
    {
        if (playerJoined)
        {
            var myPlayer = PlayroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            playerGameObjects[index].GetComponent<PlayerController>().Move();

            if (Input.GetKeyDown(KeyCode.R) && PlayroomKit.IsHost())
            {
                PlayroomKit.ResetStates(null, () =>
                {
                    var defscore = PlayroomKit.GetState<int>("score");
                    scoreText.text = "Score: " + defscore.ToString();
                });

            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                PlayroomKit.RpcRegister("playTurn", playTurnRegisterCallback, "PlayTurn Response");
                // PlayroomKit.RpcRegister("playTurn2");
                PlayroomKit.RpcRegister("playTurn3", playTurnRegisterCallback);
            }


            if (Input.GetKeyDown(KeyCode.H))
            {
                PlayroomKit.RpcCall("playTurn", "rock", PlayTurn);
                PlayroomKit.RpcCall("playTurn3", 69, PlayTurn3);
                PlayroomKit.RpcCall("playTurn", states, PlayTurn2);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                PlayroomKit.RpcCall("playTurn", playerGameObjects[index].transform.position, PlayTurn2);
            }

            players[index].SetState("posX", playerGameObjects[index].GetComponent<Transform>().position.x);
            players[index].SetState("posY", playerGameObjects[index].GetComponent<Transform>().position.y);

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

            if (PlayroomKit.IsHost())
            {
                if (playerGameObjects[i].GetComponent<Transform>().position.x >= 0f)
                {
                    score += 10;
                    scoreText.text = "Score: " + score.ToString();
                    PlayroomKit.SetState("score", score);
                }
            }
            else
            {
                scoreText.text = "Score: " + PlayroomKit.GetState<int>("score").ToString();
            }
        }

    }

    void playTurnRegisterCallback(string data, string sender)
    {
        Debug.Log("sender: " + sender);
        var player = PlayroomKit.GetPlayer(sender);
        Debug.Log("Name of sender: " + player.GetProfile().name);
    }

    void PlayTurn()
    {
        Debug.Log("playTurn called");
    }

    void PlayTurn2()
    {
        Debug.Log("playTurn2 called");
    }

    void PlayTurn3()
    {
        Debug.Log("playTurn3 called");
    }

    public static void AddPlayer(PlayroomKit.Player player)
    {
        GameObject playerObj = (GameObject)Instantiate(Resources.Load("Player"),
            new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0), Quaternion.identity);

        playerObj.GetComponent<SpriteRenderer>().color = player.GetProfile().color;
        Debug.Log(player.GetProfile().name + " Joined the game!" + "id: " + player.id);

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);

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