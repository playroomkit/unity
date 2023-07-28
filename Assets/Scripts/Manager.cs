using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AOT;

public class Manager : MonoBehaviour
{

    // public GameObject playerPrefab;

    static bool coinInserted = false;
    bool playRoomLoaded = false;

    bool playerJoined = false;


    int a = 0;
    int b = 0;

    [SerializeField] private Text text;


    Dictionary<string, int> myDictionary;


    void Awake()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.LoadPlayroom();
            playRoomLoaded = true;

        }
        else
        {
            playRoomLoaded = true;
            Debug.LogWarning("Playroom Loaded!");
        }

    }

    void Start()
    {
        if (playRoomLoaded)
        {
            Debug.Log(playRoomLoaded);
            PlayroomKit.InsertCoin(CallBackInsertCoin);
        }


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
        if (coinInserted && !playerJoined)
        {
            playerJoined = true;
            Debug.Log("Player Joined!");
            PlayroomKit.OnPlayerJoin(OnPlayerJoinCallback);
        }

    }

    [MonoPInvokeCallback(typeof(Action))]
    public static void CallBackInsertCoin()
    {
        coinInserted = true;
        Debug.Log("Insert Coin Callback Fired from Javascript defined in Unity: " + coinInserted);
    }


    [MonoPInvokeCallback(typeof(Action))]
    public static void OnPlayerJoinCallback()
    {

        Instantiate(Resources.Load("player"), new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5), 0), Quaternion.identity);
        Debug.Log("Player Instantiated!");
    }


    // for buttons
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);

        PlayroomKit.SetState("myDictionary", myDictionary);

        PlayroomKit.SetState("valX", a);

        PlayroomKit.SetState("floatKey", 3.14f);

        PlayroomKit.SetState("stringKey", "hello");

        PlayroomKit.SetState("boolKey", true);



        text.text = "a = " + a + " b = " + b;
    }

    public void TestGetState()
    {
        Debug.Log("b = " + b);

        b = PlayroomKit.GetStateInt("valX");

        Debug.Log("Getting a float: " + PlayroomKit.GetStateFloat("floatKey"));

        Debug.Log("Getting a bool: " + PlayroomKit.GetStateBool("boolKey"));

        Debug.Log("Getting a String: " + PlayroomKit.GetStateString("stringKey"));

        text.text = "new b = " + b;
    }

}
