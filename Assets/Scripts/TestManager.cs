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
        PlayroomKit.InsertCoin(() =>
        {
            Debug.Log("Insert Coin Callback Fired from Javascript defined in Unity: " + coinInserted);
            PlayroomKit.OnPlayerJoin(PlayerCallback);
        }, new PlayroomKit.InitOptions()
        {
            streamMode = true
        });
    
    }

    void Start()
    {
        text.text = "a = " + a + " b = " + b;

        Debug.Log("Is Host: "+ PlayroomKit.IsHost());
        
        Debug.Log(PlayroomKit.IsStreamMode());
    }


    static void Foo()
    {
        Debug.Log("Foo Called");
    }

    public static void PlayerCallback(PlayroomKit.Player player)
    {
        myPlayer = player;
        // spawn a player in the scene
        
        myPlayer.SetState("Score", 10, true);
        
        playerObj = (GameObject)Instantiate(Resources.Load("player"), new Vector3(-4, 4, 0), Quaternion.identity);
        // myPlayer.OnQuit(Foo);
    }
    
    public void GetProfile()
    {
        var myProfile = myPlayer.GetProfile();

        Debug.Log(myProfile.name);
        
        ColorUtility.TryParseHtmlString(myProfile.color.hexString, out Color color1);
        playerObj.GetComponent<SpriteRenderer>().color = color1;
        
    }
    
    // // for buttons
  
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);
        
        
        PlayroomKit.SetState("test", a, true);
        text.text = "a = " + a + " b = " + b;

        Dictionary<string, float> currenPos = new()
        {
            { "x", playerObj.transform.position.x},
            { "y", playerObj.transform.position.y}
        };

        PlayroomKit.SetState("pos", currenPos);
        
    }


    static void bar()
    {
        Debug.Log("Bar Called");
    }
    
    public void TestGetState()
    {

        b = PlayroomKit.GetState<int>("test");
        
        // int c = myPlayer.GetStateInt("score");
        // Debug.Log("score  " + c);
            
        text.text = "a = " + a + " b = " + b;

         Dictionary<string, float> newPosDic = PlayroomKit.GetStateDict<float>("pos");
        
         Vector3 newPos = new Vector3(newPosDic["x"], newPosDic["y"], 0);

         playerObj.transform.position = newPos;
         
         // PlayroomKit.Player plr  = PlayroomKit.Me();

    }

}
