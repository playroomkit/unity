using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AOT;



public class Manager : MonoBehaviour
{

    static bool coinInserted = false;

    int a = 0;
    int b = 0;

    public static string playerID;

    [SerializeField] private Text text;


    Dictionary<string, int> myDictionary;

    static GameObject myplayer;


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
        myDictionary = new Dictionary<string, int>
        {
            { "x", 100 },
            { "y", 200 },
            { "z", 300 }
        };

        text.text = "a = " + a + " b = " + b;

    }

    public void GetProfile()
    {
        string hexColor = PlayroomKit.Player.GetProfileByPlayerId(playerID);

        Debug.Log("Getting this hexColor: " + hexColor);

        ColorUtility.TryParseHtmlString(hexColor, out Color color1);
        myplayer.GetComponent<SpriteRenderer>().color = color1;
    }



    [MonoPInvokeCallback(typeof(Action))]
    public static void CallBackInsertCoin()
    {
        Debug.Log("Insert Coin Callback Fired from Javascript defined in Unity: " + coinInserted);
        PlayroomKit.OnPlayerJoin(PlayerCallback);
    }

    public static void PlayerCallback(PlayroomKit.Player player)
    {

        Debug.Log("EXTRA CALLBACK: " + player.playerId);

        player.SetState("score", 0);


        // spawn a player in the scene
        myplayer = (GameObject)Instantiate(Resources.Load("player"), new Vector3(0, 0, 0), Quaternion.identity);
    }


    // for buttons
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);

        text.text = "a = " + a + " b = " + b;
    }

    public void TestGetState()
    {
        Debug.Log("b = " + b);


        // // Debug.Log("GETING FLOAT: " + PlayroomKit.GETFloat());

        // Debug.Log("Getting a int: " + PlayroomKit.Player.GetPlayerStateIntById(playerID, "score"));

        // Debug.Log("Getting a float: " + PlayroomKit.Player.GetPlayerStateFloatById(playerID, "abc"));

        // Debug.Log("Getting a bool: " + PlayroomKit.Player.GetPlayerStateBoolById(playerID, "bool"));

        // Debug.Log("Getting a String: " + PlayroomKit.Player.GetPlayerStateStringById(playerID, "string"));


        // b = PlayroomKit.Player.GetPlayerStateIntById(playerID, "score");



        text.text = "new b = " + b;
    }

}
