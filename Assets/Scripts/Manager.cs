using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AOT;
using System.Runtime.InteropServices;

public class Manager : MonoBehaviour
{

    static bool coinInserted = false;

    int a = 0;
    int b = 0;

    public static string playerID;

    [SerializeField] private Text text;


    Dictionary<string, int> myDictionary;

    static GameObject player;


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

    void Update()
    {

    }

    public void GetProfile()
    {
        string hexColor = PlayroomKit.GetProfileByPlayerId(playerID);

        Debug.Log("Getting this hexColor: " + hexColor);

        ColorUtility.TryParseHtmlString(hexColor, out Color color1);
        player.GetComponent<SpriteRenderer>().color = color1;
    }



    [MonoPInvokeCallback(typeof(Action))]
    public static void CallBackInsertCoin()
    {
        Debug.Log("Insert Coin Callback Fired from Javascript defined in Unity: " + coinInserted);
        PlayroomKit.OnPlayerJoin(CallbackOnPlayerJoin);
    }



    [MonoPInvokeCallback(typeof(Action<string>))]
    public static void CallbackOnPlayerJoin(string id)
    {


        playerID = id;
        Debug.Log("Getting this playerID: " + playerID);
        // load Resources.Load("player") 


        player = (GameObject)Instantiate(Resources.Load("player"), new Vector3(0, 0, 0), Quaternion.identity);
    }


    // for buttons
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);

        PlayroomKit.SetState("myDictionary", myDictionary);

        PlayroomKit.SetState("valX", a);


        PlayroomKit.SetState("stringKey", "hello");

        PlayroomKit.SetState("boolKey", true);



        text.text = "a = " + a + " b = " + b;
    }

    public void TestGetState()
    {
        Debug.Log("b = " + b);

        b = PlayroomKit.GetStateInt("valX");

        Debug.Log("Getting a bool: " + PlayroomKit.GetStateBool("boolKey"));

        Debug.Log("Getting a String: " + PlayroomKit.GetStateString("stringKey"));

        text.text = "new b = " + b;
    }

}
