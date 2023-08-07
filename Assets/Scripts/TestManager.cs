using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AOT;
using Playroom;


public class TestManager : MonoBehaviour
{

    static bool coinInserted = false;

    int a = 0;
    int b = 0;

    public static string playerID;

    [SerializeField] private Text text;


    Dictionary<string, float> myDictionary;

    static GameObject myplayer;

    PlayroomKit.Player player;


    void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            coinInserted = true;
            PlayroomKit.InsertCoin(CallBackInsertCoin);

        }
        else
        {
            // coinInserted = true;
            Debug.LogWarning("Playroom Loaded!");
        }
    }

    void Start()
    {
        PlayroomKit.OnPlayerJoin(PlayerCallback);
        text.text = "a = " + a + " b = " + b;

    }

   



    [MonoPInvokeCallback(typeof(Action))]
    public static void CallBackInsertCoin()
    {
        Debug.Log("Insert Coin Callback Fired from Javascript defined in Unity: " + coinInserted);
        
    }

    public static void PlayerCallback(PlayroomKit.Player player)
    {
        // spawn a player in the scene
        myplayer = (GameObject)Instantiate(Resources.Load("player"), new Vector3(-4, 4, 0), Quaternion.identity);
    }
    
    /*public void GetProfile()
    {
        string hexColor = PlayroomKit.Player.GetProfileByPlayerId(playerID);
    
        Debug.Log("Getting this hexColor: " + hexColor);
    
        ColorUtility.TryParseHtmlString(hexColor, out Color color1);
        myplayer.GetComponent<SpriteRenderer>().color = color1;
    }
    
    
    // // for buttons
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);
        
        
        // Debug.Log("Getting score for the Player = " + player.SetState(playerID, "score"));
        
        text.text = "a = " + a + " b = " + b;
    }
    
    
    public void TestGetState()
    {
        Debug.Log("b = " + b);
    
        Dictionary<string, float> newPos = player.GetStateFloat(player.id, "position");
    
        Debug.Log("Getting POSX = " + newPos["x"]);
        Debug.Log("Getting POSY = " + newPos["y"]);
        Debug.Log("Getting POSZ = " + newPos["z"]);
    
        text.text = "new b = " + b;
    }*/

}
