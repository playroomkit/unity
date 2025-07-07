using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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


    List<Mapping> mappings = new()
    {
            new Mapping() { Prefix = "json", Target = "jsonplaceholder.typicode.com", }
    };

    #endregion

    #region Custom Classes
    [Serializable]
    public class CustomMetadataClass
    {
        public string packId;
    }
    #endregion

    #region Unity Lifecycle

    IEnumerator GetRequest(string url, Action<string> onComplete)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
                onComplete?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                onComplete?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }


    void Awake()
    {
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

    public IEnumerator GrantServerReward(
    string gameId,
    string jwtToken,
    string gameApiKey,
    string rewardId,
    Action<string> onSuccess,
    Action<string, string> onError
)
    {
        string url = $"{baseUrl}/discord/server-rewards?gameId={UnityWebRequest.EscapeURL(gameId)}";

        var bodyJson = $"{{\"rewardId\":\"{rewardId}\"}}";

        Debug.LogWarning($"body: {bodyJson}");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
        using (var request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            request.SetRequestHeader("x-game-api", gameApiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                onError?.Invoke($"Error granting reward: {request.error}", request.downloadHandler.text);
                yield break;
            }

            long code = request.responseCode;
            string text = request.downloadHandler.text;

            switch (code)
            {
                case 200:
                    onSuccess?.Invoke(text);
                    break;

                case 409:
                    onError?.Invoke("Reward already granted.", request.downloadHandler.text);
                    break;

                case 404:
                    onError?.Invoke("Server not yet joined. Please direct the player to join the Discord server first.", request.downloadHandler.text);
                    break;

                case 400:
                    onError?.Invoke("Invalid request. Check that gameId and rewardId are correct.", request.downloadHandler.text);
                    break;

                case 500:
                    onError?.Invoke("Server-side error. Please retry or contact support: " + request.error, request.downloadHandler.text);
                    break;

                default:
                    onError?.Invoke($"Unexpected response ({code}): {text}", request.downloadHandler.text);
                    break;
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

            Debug.LogWarning("Patching Discord URL Mappings");

            playroomKit.PatchDiscordUrlMappings(new()
            {
                new Mapping() { Prefix = ".proxy/json", Target = "jsonplaceholder.typicode.com", },
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

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(GetRequest("https://jsonplaceholder.typicode.com/todos/1", (response) =>
            {
                text.text = "Response from JSON Placeholder: " + response;
                Debug.Log("Response: " + response);
            }));
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