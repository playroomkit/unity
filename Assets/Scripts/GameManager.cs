using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;
using Playroom;
using Unity.VisualScripting;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;


public class GameManager : MonoBehaviour
{
    private static List<PlayroomKit.Player> players = new();
    private static Object playerObj;
    private static Transform playerTransform;



    private void Start()
    {
        PlayroomKit.InsertCoin(() =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
        });
    }

    private void Update()
    {
        if (PlayroomKit.IsHost())
        {

            // TODO: controls 
            
            foreach (var player in players)
            {
                player.SetState("pos", (int)playerTransform.position.x);
            }
        }
        else
        {
            foreach (var player in players)
            {
                var pos = player.GetStateInt("pos");
                var position = playerTransform.position;
                position = new Vector3(pos, position.y, position.z);
                playerTransform.position = position;
            }
        }
    }

    public static void AddPlayer(PlayroomKit.Player player)
    {
        players.Add(player);
        playerObj = Instantiate(Resources.Load("Player"), new Vector3(-4, 4, 0), Quaternion.identity);
        
        var myProfile = player.GetProfile();
        ColorUtility.TryParseHtmlString(myProfile.color.hexString, out Color color1);
        playerObj.GetComponent<SpriteRenderer>().color = color1;
        
        playerTransform = playerObj.GetComponent<Transform>().transform;
    }
    

    
}