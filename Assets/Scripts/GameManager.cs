using System.Collections.Generic;
using Playroom;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();

    private static readonly List<string> playerID = new();
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
            var pos = players[i].GetState<float>("pos");
            if (playerGameObjects != null)
                playerGameObjects[i].GetComponent<Transform>().position = new Vector3(
                    pos, playerGameObjects[i].GetComponent<Transform>().position.y, 0f);

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
        Debug.Log(player.GetProfile().name + " Joined the game!");
        
        
        string playerId = player.id;
        
        player.OnQuit((() =>
        {
            PlayroomKit.Player playerWhoLeft = players.Find((player) => player.id == playerId);
            Debug.LogWarning(  playerWhoLeft.GetProfile().name + " Left the game");
            Destroy(playerObj);
            // var newPlayer = PlayroomKit.MyPlayer();
            // var index = players.IndexOf(newPlayer);
            // Destroy(playerGameObjects[index]);
        }));

       
        playerID.Add(player.id);
        players.Add(player);
        playerGameObjects.Add(playerObj);
        
        
        
    }

}