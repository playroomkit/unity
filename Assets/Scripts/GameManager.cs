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
    #region Fields
    [SerializeField] private string gameId = "FmOBeUfQO2AOLNIrJNSJ";
    [SerializeField] private string baseUrl = "https://ws.joinplayroom.com/api/";
    [SerializeField] private string gameApiKey;

    [SerializeField]
    private string token = "eyJhbGciOiJIUzI1NiJ9.eyJkaXNjb3JkSWQiOiI0NzY3MDk1MjQwMTE2MTQyMTkiLCJyb29tSWQiOiJEQ1JEX2ktMTM3NDY3NDQ2MDUyMjcxMzE1OS1nYy0xMjczNjA3Njg2ODE4MTA3NDAyLTEyNzQ5OTUxNDcyNjc4OTk0NTYiLCJnYW1lSWQiOiJGbU9CZVVmUU8yQU9MTklySk5TSiIsImd1aWxkSWQiOiIxMjczNjA3Njg2ODE4MTA3NDAyIiwiY2hhbm5lbElkIjoiMTI3NDk5NTE0NzI2Nzg5OTQ1NiIsImFjY2Vzc1Rva2VuIjoiV2FVNFV1RjJ6UEJOTzY1QWZISlNrR2RzaVBGOWpKIiwiYXV0aCI6ImRpc2NvcmQiLCJ0IjoxNzQ3ODE4MzYzfQ.bRvWY7SMafo0iQzfdp0N53Euu58eC35AZ6ruiCdgF0M";

    private PlayroomKit playroomKit;

    public TextMeshProUGUI text;

    bool coinInserted = false;

    string skuId = "1371921246031319121";
    #endregion

    #region HQ Entitlements
    [Header("HQ Entitlements")]
    [SerializeField]
    private List<SKU<CustomMetadataClass>> skus;

    [SerializeField]
    private List<PlayerEntitlement<CustomMetadataClass>> entitlements;
    #endregion

    #region Discord Data

    [Header("DISORD STUFF")]
    [SerializeField]
    private List<DiscordSku> discordSkus = new List<DiscordSku>();

    [SerializeField]
    private List<DiscordEntitlement> discordEntitlements = new List<DiscordEntitlement>();

    [SerializeField]
    private List<ServerReward> serverRewards = new();

    #endregion


    #region Debug UI
    [Header("Debug UI")]
    private bool showDebugWindow = false;
    private bool showHQDebugWindow = false;
    private Vector2 scrollPosition;
    private Vector2 hqScrollPosition;
    private string debugText = "";
    private string discordEntitlementsText = "";
    private string discordSkusText = "";
    private string hqEntitlementsText = "";
    private string hqSkusText = "";
    #endregion

    #region Custom Classes
    [Serializable]
    public class CustomMetadataClass
    {
        public string packId;
    }
    #endregion

    #region Unity Lifecycle



    void Awake()
    {
        if (Application.absoluteURL.Contains("discord"))
        {
            baseUrl = ".proxy/_ws/api";
        }
        else
        {
            baseUrl = "https://ws.joinplayroom.com/api";
        }

        // Initialize fake Discord SKUs
        discordSkus = new List<DiscordSku>
        {
            new DiscordSku
            {
                Id = "premium_pack_1",
                Name = "Premium Pack",
                Type = DiscordSkuType.APPLICATION,
                ApplicationId = "123456789",
                Price = new DiscordSkuPrice { Amount = 999, Currency = "USD" }
            },
            new DiscordSku
            {
                Id = "starter_pack",
                Name = "Starter Pack",
                Type = DiscordSkuType.APPLICATION,
                ApplicationId = "123456789",
                Price = new DiscordSkuPrice { Amount = 499, Currency = "USD" }
            },
            new DiscordSku
            {
                Id = "deluxe_edition",
                Name = "Deluxe Edition",
                Type = DiscordSkuType.APPLICATION,
                ApplicationId = "123456789",
                Price = new DiscordSkuPrice { Amount = 1999, Currency = "USD" }
            }
        };

        playroomKit = new PlayroomKit();
    }

    private void Start()
    {
        playroomKit.InsertCoin(new InitOptions()
        {
            gameId = "FmOBeUfQO2AOLNIrJNSJ",
            maxPlayersPerRoom = 2,
            discord = new DiscordOptions()
            {
                Scope = new() { "applications.commands", "guilds" }
            },

            // discord = true
        }, OnLaunchCallBack);
    }
    public IEnumerator GetActiveServerRewards(
        string gameId,
        string jwtToken,
        string gameApiKey,
        Action<string> onSuccess,
        Action<string> onError
    )
    {
        // Build full URL with query parameter
        string url = $"{baseUrl}/discord/server-rewards?gameId={UnityWebRequest.EscapeURL(gameId)}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            request.SetRequestHeader("x-game-api", gameApiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke($"Error fetching server rewards: {request.error}");
            }
            else
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            showDebugWindow = !showDebugWindow;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            showHQDebugWindow = !showHQDebugWindow;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            FetchEntitlements((data) =>
            {
                text.text = "Entitlements fetched successfully!";
                entitlements = PlayerEntitlement<CustomMetadataClass>.FromJSON(data, ParseMetadata);
            });
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            FetchSKUS((jsonString) =>
            {
                text.text = "SKUs fetched successfully!";
                skus = SKU<CustomMetadataClass>.FromJSON(jsonString, ParseMetadata);
            });
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            playroomKit.StartDiscordPurchase(skuId, (response) =>
            {
                discordEntitlements = DiscordEntitlement.FromJSON(response);
                debugText = "Purchase completed!\n" + response;
            }, (onErrorResponse) =>
            {
                text.text = onErrorResponse;
            });
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            playroomKit.GetDiscordSkus((data) =>
            {
                text.text = "Discord SKUs fetched successfully!";

                data.ForEach((data) =>
                {
                    Debug.Log($"UNITY: Discord Sku: {data.Id}, {data.Name}");
                });

                discordSkus = data;
            });
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            playroomKit.GetDiscordEntitlements((entitlements) =>
            {
                text.text = "Discord entitlements fetched successfully!";
                discordEntitlements = entitlements;
            });
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            text.text = "";
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            discordSkus.ForEach((sku) =>
            {
                Debug.LogWarning($"{sku.Name}: {sku.Price.Amount}, {sku.Price.Currency}");
                playroomKit.DiscordFormatPrice(sku.Price, "en-US", (formattedPrice) => text.text += $"{sku.Name} - {formattedPrice}");
            });
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            playroomKit.OpenDiscordExternalLink("https://github.com/momintlh", (success) =>
            {
                text.text = success;
            });
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            playroomKit.OpenDiscordExternalLink("https://discord.gg/9s5jfcgQ6P", (success) =>
            {
                text.text = success;
            });
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(GetActiveServerRewards(gameId, "eyJhbGciOiJIUzI1NiJ9.eyJkaXNjb3JkSWQiOiI0NzY3MDk1MjQwMTE2MTQyMTkiLCJyb29tSWQiOiJEQ1JEX2ktMTM4OTUzMzUzNjg5NzIwNDMxNC1wYy0xMzcxOTI3NzM0MTY2NDg3MDkwIiwiZ2FtZUlkIjoiRm1PQmVVZlFPMkFPTE5JckpOU0oiLCJndWlsZElkIjpudWxsLCJjaGFubmVsSWQiOiIxMzcxOTI3NzM0MTY2NDg3MDkwIiwiYWNjZXNzVG9rZW4iOiJNVE0zTURReE5qSTRORFk0T0RNeU1qWTRNZy5XUVFnOFp5bjNHNnNuYVJTR21qQmZDM1A4bm5tcm4iLCJhdXRoIjoiZGlzY29yZCIsInQiOjE3NTEzNjEwNDJ9.wXL2lVdyCyXNbiNjQGrFV4bqoESt3eLyflAMt50evY8", "510a71af-3a69-4f5d-9b9b-296a1871e624", (result) =>
            {
                text.text = result;
                serverRewards = ServerReward.FromJSON(result);
            }, (error) => text.text = error));
        }

    }
    #endregion

    #region API Methods
    public void FetchSKUS(Action<string> onRequestComplete = null)
    {
        StartCoroutine(GetSKUS(onRequestComplete));
    }

    private IEnumerator GetSKUS(Action<string> onRequestComplete = null)
    {
        var url = $"{baseUrl}/store/sku?gameId={UnityWebRequest.EscapeURL("FmOBeUfQO2AOLNIrJNSJ")}&platform={UnityWebRequest.EscapeURL("discord")}";

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
        var url = $"{baseUrl}/store/entitlement?gameId={UnityWebRequest.EscapeURL("FmOBeUfQO2AOLNIrJNSJ")}";

        using (var req = UnityWebRequest.Get(url))
        {
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
    #endregion

    #region Playroom Callbacks
    private void OnLaunchCallBack()
    {
        playroomKit.OnPlayerJoin(CreatePlayer);
        coinInserted = true;
        playroomKit.SubscribeDiscordEvent(SDKEvent.ENTITLEMENT_CREATE, (data) =>
        {
            Debug.LogWarning("DATA UNITY: " + data);
            text.text = data;
        });
    }

    private void CreatePlayer(PlayroomKit.Player player)
    {
        Debug.Log($"{player.id} joined the room!");

    }
    #endregion

    #region Helper Methods
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
    #endregion

    #region Debug UI Methods
    void OnGUI()
    {
        // Instructions
        GUI.Label(new Rect(10, 10, 400, 200),
            "Press E - Fetch Entitlements\n" +
            "Press S - Fetch SKUs\n" +
            "Press P - Start Discord Purchase\n" +
            "Press T - Get Discord SKUs\n" +
            "Press D - Get Discord Entitlements\n" +
            "Press F3 - Toggle Discord Debug Window\n" +
            "Press F2 - Toggle HQ Debug Window\n" +
            "Press F - Format SKU Price");

        // Discord Debug Window
        if (showDebugWindow)
        {
            GUI.Window(0, new Rect(10, 220, 600, 400), DrawDiscordDebugWindow, "Discord Debug Information");
        }

        // HQ Debug Window
        if (showHQDebugWindow)
        {
            GUI.Window(1, new Rect(620, 220, 600, 400), DrawHQDebugWindow, "HQ Debug Information");
        }
    }

    void DrawDiscordDebugWindow(int windowID)
    {
        scrollPosition = GUI.BeginScrollView(new Rect(10, 20, 580, 370), scrollPosition, new Rect(0, 0, 560, 2000));
        GUILayout.BeginVertical();

        GUILayout.Label("Discord Entitlements:", EditorLabelBold());
        if (discordEntitlements != null)
        {
            foreach (var entitlement in discordEntitlements)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                DrawInspectorField("Id", entitlement.Id);
                DrawInspectorField("SKU Id", entitlement.SkuId);
                DrawInspectorField("Type", entitlement.Type);
                DrawInspectorField("Application Id", entitlement.ApplicationId);
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("Discord SKUs:", EditorLabelBold());
        if (discordSkus != null)
        {
            foreach (var sku in discordSkus)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                DrawInspectorField("Id", sku.Id);
                DrawInspectorField("Name", sku.Name);
                if (sku.Price != null)
                {
                    GUILayout.Label("Price", EditorLabelBold());
                    GUILayout.BeginVertical(GUI.skin.box);
                    DrawInspectorField("Amount", sku.Price.Amount.ToString());
                    DrawInspectorField("Currency", sku.Price.Currency);
                    GUILayout.EndVertical();
                }
                DrawInspectorField("Type", sku.Type.ToString());
                DrawInspectorField("Application Id", sku.ApplicationId);
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
        }

        GUILayout.EndVertical();
        GUI.EndScrollView();
    }

    void DrawInspectorField(string label, string value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(120));
        GUILayout.TextField(value ?? string.Empty, GUILayout.Width(400));
        GUILayout.EndHorizontal();
    }

    void DrawHQDebugWindow(int windowID)
    {
        hqScrollPosition = GUI.BeginScrollView(new Rect(10, 20, 580, 370), hqScrollPosition, new Rect(0, 0, 560, 2000));
        GUILayout.BeginVertical();

        GUILayout.Label("HQ Entitlements:", EditorLabelBold());
        if (entitlements != null)
        {
            foreach (var entitlement in entitlements)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                DrawInspectorField("Id", entitlement.id);
                DrawInspectorField("SKU Id", entitlement.skuId);
                DrawInspectorField("Type", entitlement.type);
                if (entitlement.metadata != null)
                {
                    GUILayout.Label("Metadata", EditorLabelBold());
                    GUILayout.BeginVertical(GUI.skin.box);
                    DrawInspectorField("Pack Id", entitlement.metadata.packId);
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("HQ SKUs:", EditorLabelBold());
        if (skus != null)
        {
            foreach (var sku in skus)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                DrawInspectorField("Id", sku.id);
                DrawInspectorField("Name", sku.name);
                DrawInspectorField("Description", sku.description);
                DrawInspectorField("Type", sku.type);
                DrawInspectorField("Image", sku.image);
                DrawInspectorField("Key", sku.key);
                DrawInspectorField("Active", sku.active.ToString());
                DrawInspectorField("Deleted", sku.deleted.ToString());
                if (sku.metadata != null)
                {
                    GUILayout.Label("Metadata", EditorLabelBold());
                    GUILayout.BeginVertical(GUI.skin.box);
                    DrawInspectorField("Pack Id", sku.metadata.packId);
                    GUILayout.EndVertical();
                }
                DrawInspectorField("Price", sku.price.ToString());
                DrawInspectorField("Product Id", sku.productId);
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
        }

        GUILayout.EndVertical();
        GUI.EndScrollView();
    }

    GUIStyle EditorLabelBold()
    {
        var style = new GUIStyle(GUI.skin.label);
        style.fontStyle = FontStyle.Bold;
        return style;
    }
    #endregion


}