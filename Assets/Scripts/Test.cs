using UnityEngine;
using Playroom;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Discord;
using System;


public class Test : MonoBehaviour
{
    PlayroomKit prk;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI responseText;

    // Using Playroomkit for setting up discord auth and hosting the activity.    PlayroomKit prk;
    bool discordReady;

    // Creating the mappings for the external resources, we will need to add this in discord developer portal as well check : https://discord.com/developers/docs/activities/development-guides#using-external-resources
    List<Mapping> mappings = new()
    {
        new Mapping() { Prefix = "/.proxy/json", Target = "jsonplaceholder.typicode.com", },
        new Mapping() { Prefix = "/.proxy/_ws", Target = "https://ws.joinplayroom.com", }
    };

    void Awake()
    {
        prk = new();
        prk.PatchDiscordUrlMappings(mappings);
    }

    void Start()
    {
        prk.InsertCoin(new()
        {
            // gameId = "cW0r8UJ1aXnZ8v5TPYmv",
            gameId = "FmOBeUfQO2AOLNIrJNSJ",
            discord = true,
        }, () =>
        {
            Debug.LogWarning("Coin Inserted");
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            APICall();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(GetSKUS((response) =>
            {
                Debug.LogWarning("SKUs: " + response);
                responseText.text = response;
            }));
        }
    }

    #region API Call
    private void APICall()
    {
        StartCoroutine(GetRequest("https://jsonplaceholder.typicode.com/todos/1"));
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
                responseText.text = webRequest.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                responseText.text = webRequest.error;
            }
        }
    }

    private IEnumerator GetSKUS(Action<string> onRequestComplete = null)
    {
        // var url = $"https://ws.joinplayroom.com/api/store/sku?gameId={UnityWebRequest.EscapeURL("FmOBeUfQO2AOLNIrJNSJ")}&platform={UnityWebRequest.EscapeURL("discord")}";
        var url = $"/.proxy/_ws/api/store/sku?gameId={UnityWebRequest.EscapeURL("FmOBeUfQO2AOLNIrJNSJ")}&platform={UnityWebRequest.EscapeURL("discord")}";

        using (var req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("x-game-api", "510a71af-3a69-4f5d-9b9b-296a1871e624");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching SKUs: {req.error}");
            }
            else
            {
                onRequestComplete?.Invoke(req.downloadHandler.text);
            }
        }
    }
    #endregion
}