using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();

    private static Dictionary<string, GameObject> PlayerDict = new();
    Dictionary<string, float> moveData = new();

    [SerializeField] private int score;
    [SerializeField] private Text scoreText;

    [SerializeField] private static bool playerJoined;


    private void Awake()
    {

        PlayroomKit.OnPlayerJoin(AddPlayer);

        // PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        // {
        //     defaultPlayerStates = new() {
        //                 {"score", -500},
        //             }
        // }, () =>
        // {
        //     PlayroomKit.OnPlayerJoin(AddPlayer);
        //     PlayroomKit.SetState("score", score);

        // });

    }


    private void Update()
    {
        if (playerJoined)
        {
            var myPlayer = PlayroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            playerGameObjects[index].GetComponent<PlayerController>().Move();

            if (Input.GetKeyDown(KeyCode.L) && PlayroomKit.IsHost())
            {
                PlayroomKit.ResetStates(null, () =>
                {
                    var defscore = PlayroomKit.GetState<int>("score");
                    scoreText.text = "Score: " + defscore.ToString();
                });
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
                Debug.Log(PlayroomKit.GetState<int>("score"));
                scoreText.text = "Score: " + score.ToString();
            }
        }

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