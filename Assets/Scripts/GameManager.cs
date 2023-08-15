using System.Collections.Generic;
using UnityEngine;
using Playroom;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;


public class GameManager : MonoBehaviour
{
    private static List<PlayroomKit.Player> players = new();
    private static GameObject playerObj;
    private static Transform playerTransform;
    
    private static PlayerController playerContoller;


    private void Start()
    {
        PlayroomKit.InsertCoin(() =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
        });
    }

    private void Update()
    {
        playerContoller.Move();
        if (PlayroomKit.IsHost())
        {
            foreach (var player in players)
            {
                player.SetState("pos", playerTransform.position.x);
            }
        }
        else
        {
            foreach (var player in players)
            {
                var pos = player.GetState<float>("pos");
                var position = playerTransform.position;
                position = new Vector3(pos, position.y, position.z);
                playerTransform.position = position;
            }
        }
    }

    public static void AddPlayer(PlayroomKit.Player player)
    {

    
        
        players.Add(player);
        playerObj = (GameObject)Instantiate(Resources.Load("Player"), new Vector3(-4, 4, 0), Quaternion.identity);
        
        var myProfile = player.GetProfile();
        ColorUtility.TryParseHtmlString(myProfile.color.hexString, out Color color1);
        playerObj.GetComponent<SpriteRenderer>().color = color1;
        
        playerTransform = playerObj.GetComponent<Transform>().transform;
        playerContoller = playerObj.GetComponent<PlayerController>();
        
    }


    
}