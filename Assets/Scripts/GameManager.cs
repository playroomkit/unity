using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();


    private static Dictionary<string, GameObject> PlayerDict = new(); 


    [SerializeField] private int score = 0;
   


    private void Awake()
    {
        PlayroomKit.InsertCoin(() =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
            PlayroomKit.SetState("score", score);
        });
    }

    private void Update()
    {

        
        var myPlayer = PlayroomKit.MyPlayer();
        var index = players.IndexOf(myPlayer);

         playerGameObjects[index].GetComponent<PlayerController>().Move();
         players[index].SetState("pos", playerGameObjects[index].GetComponent<Transform>().position.x);

        for (var i = 0; i < players.Count; i++)
        {

            if (players[i] != null){
                var pos = players[i].GetState<float>("pos");
                if (playerGameObjects != null)
                    playerGameObjects[i].GetComponent<Transform>().position = new Vector3(
                        pos, playerGameObjects[i].GetComponent<Transform>().position.y, 0f);
            }

            if (PlayroomKit.IsHost())
            {
                if (playerGameObjects[i].GetComponent<Transform>().position.x >= 0f)
                {
                    score += 10;
                    PlayroomKit.SetState("score", score);
                }
            }
            else
            {
                Debug.Log(PlayroomKit.GetState<int>("score"));
            }
            
        }
    }

    public static void AddPlayer(PlayroomKit.Player player)
    {
        GameObject playerObj = (GameObject)Instantiate(Resources.Load("Player"),
            new Vector3(Random.Range(-4, 4), Random.Range(1, 5), 0), Quaternion.identity);
        
        
        playerObj.GetComponent<SpriteRenderer>().color = player.GetProfile().color;
        Debug.Log(player.GetProfile().name + " Joined the game!" + "id: " +  player.id);

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);


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