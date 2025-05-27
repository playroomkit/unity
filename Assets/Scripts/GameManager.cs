using System;
using System.Collections;
using System.Collections.Generic;
using Discord;
using Playroom;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string baseUrl = "https://ws.joinplayroom.com/api/store";
    [SerializeField] private string gameApiKey;

    [SerializeField]
    private string token = "eyJhbGciOiJIUzI1NiJ9.eyJkaXNjb3JkSWQiOiI0NzY3MDk1MjQwMTE2MTQyMTkiLCJyb29tSWQiOiJEQ1JEX2ktMTM3NDY3NDQ2MDUyMjcxMzE1OS1nYy0xMjczNjA3Njg2ODE4MTA3NDAyLTEyNzQ5OTUxNDcyNjc4OTk0NTYiLCJnYW1lSWQiOiJGbU9CZVVmUU8yQU9MTklySk5TSiIsImd1aWxkSWQiOiIxMjczNjA3Njg2ODE4MTA3NDAyIiwiY2hhbm5lbElkIjoiMTI3NDk5NTE0NzI2Nzg5OTQ1NiIsImFjY2Vzc1Rva2VuIjoiV2FVNFV1RjJ6UEJOTzY1QWZISlNrR2RzaVBGOWpKIiwiYXV0aCI6ImRpc2NvcmQiLCJ0IjoxNzQ3ODE4MzYzfQ.bRvWY7SMafo0iQzfdp0N53Euu58eC35AZ6ruiCdgF0M";

    private PlayroomKit playroomKit;

    public TextMeshProUGUI text;

    bool coinInserted = false;

    string skuId = "1371921246031319121";


    [Header("HQ Entitlements")]

    [SerializeField]
    private List<SKU<CustomMetadataClass>> skus;

    [SerializeField]
    private List<PlayerEntitlement<CustomMetadataClass>> entitlements;

    [Serializable]
    public class CustomMetadataClass
    {
        public string packId;
    }

    public void FetchSKUS(Action<string> onRequestComplete = null)
    {
        StartCoroutine(GetSKUS(onRequestComplete));
    }

    private IEnumerator GetSKUS(Action<string> onRequestComplete = null)
    {
        var url = $"{baseUrl}/sku?gameId={UnityWebRequest.EscapeURL("FmOBeUfQO2AOLNIrJNSJ")}&platform={UnityWebRequest.EscapeURL("discord")}";

        using (var req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("x-game-api", gameApiKey);
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

    public void FetchEntitlements(Action<string> onRequestComplete)
    {
        StartCoroutine(GetEntitlements(onRequestComplete));
    }

    private IEnumerator GetEntitlements(Action<string> onRequestComplete)
    {
        var url = $"{baseUrl}/entitlement?gameId={UnityWebRequest.EscapeURL("FmOBeUfQO2AOLNIrJNSJ")}";

        using (var req = UnityWebRequest.Get(url))
        {
            //TODO: THIS IS FOR TESTING ONLY, REMOVE LATER
            req.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching entitlements: {req.error}");
                onRequestComplete?.Invoke(req.error);
                yield break;
            }
            onRequestComplete?.Invoke(req.downloadHandler.text);
        }
    }

    private void Awake()
    {
        playroomKit = new PlayroomKit();
    }

    private void Start()
    {
        playroomKit.InsertCoin(new InitOptions()
        {
            gameId = "FmOBeUfQO2AOLNIrJNSJ",
            maxPlayersPerRoom = 2,
            discord = true,
        }, OnLaunchCallBack);
    }

    private void OnLaunchCallBack()
    {
        playroomKit.OnPlayerJoin(CreatePlayer);
        coinInserted = true;
    }

    private void CreatePlayer(PlayroomKit.Player player)
    {
        Debug.Log($"{player.id} joined the room!");
    }

    CustomMetadataClass ParseMetadata(string json)
    {
        JSONNode node = JSON.Parse(json);

        return new CustomMetadataClass()
        {
            packId = node["packId"],
        };
    }

    CustomMetadataClass ParseMetadataUnity(string json)
    {
        return JsonUtility.FromJson<CustomMetadataClass>(json);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Token: " + playroomKit.GetPlayroomToken());
            text.text = playroomKit.GetPlayroomToken();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            playroomKit.OpenDiscordInviteDialog(() =>
            {
                text.text = "Discord invite dialog opened!";
            });
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            FetchEntitlements((data) =>
            {
                text.text = "Entitlements fetched successfully!";
                Debug.Log("Entitlements fetched successfully!");

                entitlements = PlayerEntitlement<CustomMetadataClass>.FromJSON(data, ParseMetadata);
            });
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            FetchSKUS((jsonString) =>
            {
                text.text = "SKUs fetched successfully!";
                Debug.Log("SKUs fetched successfully!");

                skus = SKU<CustomMetadataClass>.FromJSON(jsonString, ParseMetadata);
            });
        }



        if (Input.GetKeyDown(KeyCode.P))
        {
            // After InsertCoin has fully invoked
            playroomKit.StartDiscordPurchase(skuId, (response) =>
            {
                List<DiscordEntitlement> discordEntitlements = DiscordEntitlement.FromJSON(response);
            });
        }
    }
}