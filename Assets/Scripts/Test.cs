using UnityEngine;
using Playroom;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Discord;


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
        new Mapping() { Prefix = "json", Target = "jsonplaceholder.typicode.com", }
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
            gameId = "cW0r8UJ1aXnZ8v5TPYmv",
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
    #endregion
}