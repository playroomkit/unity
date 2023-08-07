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

    static GameObject playerObj;

    static PlayroomKit.Player myPlayer;


    void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            coinInserted = true;
            PlayroomKit.InsertCoin(CallBackInsertCoin);

        }
        else
        {
            Debug.LogWarning("Playroom Loaded!");
        }
    }

    void Start()
    {
        text.text = "a = " + a + " b = " + b;
    }

   



    [MonoPInvokeCallback(typeof(Action))]
    public static void CallBackInsertCoin()
    {
        Debug.Log("Insert Coin Callback Fired from Javascript defined in Unity: " + coinInserted);
        PlayroomKit.OnPlayerJoin(PlayerCallback);
    }


    static void Foo()
    {
        Debug.Log("Foo Called");
    }

    public static void PlayerCallback(PlayroomKit.Player player)
    {
        myPlayer = player;
        // spawn a player in the scene
        playerObj = (GameObject)Instantiate(Resources.Load("player"), new Vector3(-4, 4, 0), Quaternion.identity);
        myPlayer.OnQuit(Foo);
    }
    
    public void GetProfile()
    {
       var myProfile = myPlayer.GetProfile();

        Debug.Log(myProfile.name);
        
        ColorUtility.TryParseHtmlString(myProfile.color.hexString, out Color color1);
        playerObj.GetComponent<SpriteRenderer>().color = color1;
        
    }
    
    // // for buttons
    /*
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);
        
        
        // Debug.Log("Getting score for the Player = " + player.SetState(playerID, "score"));
        
        text.text = "a = " + a + " b = " + b;
    }*/


    static void bar()
    {
        Debug.Log("Bar Called");
    }
    
    public void TestQuitCallback()
    {
        PlayroomKit.GetPlayer(myPlayer.id).OnQuit(bar);
    }

}
