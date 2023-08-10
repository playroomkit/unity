    using UnityEngine;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using AOT;
    using System;
    using SimpleJSON;
    using Random = UnityEngine;
    
    namespace Playroom
    {
        public class PlayroomKit
        {

            public static Dictionary<string, Player> Players = new();

            private const string PlayerId = "testPlayer5";
            public static Dictionary<string, object> MockDictionary = new();
            
            [System.Serializable]
            public class InitOptions
            {
                public bool streamMode = false;
                public bool allowGamepads = false;
                public string baseUrl = "";
            }

            private static Action InsertCoinCallback = null;
            
            [DllImport("__Internal")]
            private static extern void InsertCoinInternal(Action callback, string options);
            
            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeInsertCoin()
            {
                InsertCoinCallback?.Invoke();
            }

            // optional InitOptions
            public static void InsertCoin(Action callback, InitOptions options = null)
            {
                if(IsRunningInBrowser())
                {
                    InsertCoinCallback = callback;
                    string optionsJson = null;
                    if (options != null) optionsJson = SerializeInitOptions(options);
                    InsertCoinInternal(InvokeInsertCoin, optionsJson);  
                }
                else
                {
                    Debug.Log("Coin Inserted");
                    callback?.Invoke();
                }
            } 
            
            private static string SerializeInitOptions(InitOptions options)
            {
                if (options == null)
                {
                    return null;
                }

                return JsonUtility.ToJson(options);
            }

            [DllImport("__Internal")]
            private static extern void OnPlayerJoinInternal(Action<string> callback);
            
            private static Action<Player> onPlayerJoinCallback = null;

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void OnPlayerJoinWrapperCallback(string id)
            {
                Player player = GetPlayer(id);
                onPlayerJoinCallback?.Invoke(player);
            }

            public static void OnPlayerJoin(Action<Player> playerCallback)
            {
                if (IsRunningInBrowser())
                {
                    onPlayerJoinCallback = playerCallback;
                    OnPlayerJoinInternal(OnPlayerJoinWrapperCallback);
                }
                else
                {
                    Debug.Log("On Player Join");
                    Player testPlayer = GetPlayer(PlayerId);
                    playerCallback?.Invoke(testPlayer);
                }
            }  
            
            public static Dictionary<string, Player> GetPlayers()
            {
                return Players;
            }
            
            public static Player GetPlayer(string playerId)
            {
                if (IsRunningInBrowser())
                {
                    try
                    {
                        if(Players.ContainsKey(playerId)) 
                        {
                            return Players[playerId];
                        }
                        else
                        {
                            Player player = new Player(playerId);
                            Players.Add(playerId, player);
                            return player;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error in Get Player: " + e.Message);
                        throw;
                    }
                }
                else
                {
                    Player testPlayer = new Player("testPlayer5");
                    return testPlayer;
                }
                
            }

            [DllImport("__Internal")]
            private static extern bool IsHostInternal();

            public static bool IsHost()
            {
                if (IsRunningInBrowser())
                {
                    return IsHostInternal();
                }
                else
                {
                    // mock player is host
                    return true;
                }
            }
            
            [DllImport("__Internal")]
            public static extern bool IsStreamMode();

            [DllImport("__Internal")]
            private static extern string MyPlayerInternal();
            
            public static Player MyPlayer()
            {
                string id = MyPlayerInternal();
                return GetPlayer(id);
            }
            
            [DllImport("__Internal")]
            private static extern string MeInternal();
            
            public static Player Me()
            {
                string id = MeInternal();
                return GetPlayer(id);
            }

            

            [DllImport("__Internal")]
            private static extern void SetStateString(string key, string value, bool reliable = false);

            [DllImport("__Internal")]
            private static extern void SetStateInternal(string key, int value, bool reliable = false);

            [DllImport("__Internal")]
            private static extern void SetStateInternal(string key, float value, bool reliable = false);

            [DllImport("__Internal")]
            private static extern void SetStateInternal(string key, bool value, bool reliable = false);

            public static void SetState(string key, string value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateString(key, value, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
            }
            
            public static void SetState(string key, int value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateInternal(key, value, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
            }
            
            public static void SetState(string key, float value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateInternal(key, value, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
            }            
            
            public static void SetState(string key, bool value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateInternal(key, value, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
            }

            [DllImport("__Internal")]
            private static extern void SetStateDictionary(string key, string jsonValues, bool reliable = false);


            public static void SetState(string key, Dictionary<string, int> values, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateHelper(key, values, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
            }

            public static void SetState(string key, Dictionary<string, float> values, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateHelper(key, values, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
            }

            public static void SetState(string key, Dictionary<string, bool> values, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateHelper(key, values, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
            }

            public static void SetState(string key, Dictionary<string, string> values, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateHelper(key, values, reliable);
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
            }


            // GETTERS
            [DllImport("__Internal")]
            private static extern string GetStateStringInternal(string key);

            private static string GetStateString(string key)
            {
                if (IsRunningInBrowser())
                {
                    return GetStateStringInternal(key);
                }
                else
                {
                    return MockGetState<string>(key);
                }
            }

            [DllImport("__Internal")]
            private static extern int GetStateIntInternal(string key);

            public static int GetStateInt(string key)
            {
                if (IsRunningInBrowser())
                {
                    return GetStateIntInternal(key);
                }
                else
                {
                    return MockGetState<int>(key);
                }
            }
            
            [DllImport("__Internal")]
            private static extern float GetStateFloatInternal(string key);

            public static float GetStateFloat(string key)
            {
                if (IsRunningInBrowser())
                {
                    return GetStateFloat(key);
                }
                else
                {
                    return MockGetState<float>(key);
                }
            }

            public static bool GetStateBool(string key)
            {
                if (IsRunningInBrowser())
                {
                    var stateValue = GetStateInt(key);
                    return stateValue == 1 ? true :
                        stateValue == 0 ? false :
                        throw new InvalidOperationException($"GetStateBool: {key} is not a bool");
                }
                else
                {
                    return MockGetState<bool>(key);
                }

                
            }

            public static T GetState<T>(string key)
            {
                if (IsRunningInBrowser())
                {
                    if (typeof(T) == typeof(int))
                    {
                        return (T)(object)GetStateIntInternal(key);
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        return (T)(object)GetStateFloatInternal(key);
                    }
                    else if(typeof(T) == typeof(bool))
                    {
                        return (T)(object)GetStateBool(key);
                    }
                    else if(typeof(T) == typeof(string))
                    {
                        return (T)(object)GetStateString(key);
                    }
                    else
                    {
                        Debug.LogError($"GetState<{typeof(T)}> is not supported.");
                        return default;
                    }
                }
                else
                {
                    return MockGetState<T>(key);
                }
            } 
            
            
            [DllImport("__Internal")]
            private static extern string GetStateDictionaryInternal(string key);
            
            public static Dictionary<string, T> GetStateDict<T>(string key)
            {
                if (IsRunningInBrowser())
                {
                    string jsonString = GetStateDictionaryInternal(key);
                    return ParseJsonToDictionary<T>(jsonString);
                }
                else
                {
                    return MockGetState<Dictionary<string, T>>(key);
                }
            }
            
            // Utils:
            private static void SetStateHelper<T>(string key, Dictionary<string, T> values, bool reliable = false)
            {
                JSONObject jsonObject = new JSONObject();

                // Add key-value pairs to the JSON object
                foreach (var kvp in values)
                {
                    // Convert the value to double before adding to JSONNode
                    double value = Convert.ToDouble(kvp.Value);
                    jsonObject.Add(kvp.Key, value);
                }

                // Serialize the JSON object to a string
                string jsonString = jsonObject.ToString();

                // Output the JSON string
                SetStateDictionary(key, jsonString, reliable);
            }

            private static Dictionary<string, T> ParseJsonToDictionary<T>(string jsonString)
            {
                Dictionary<string, T> dictionary = new Dictionary<string, T>();
                JSONNode jsonNode = JSON.Parse(jsonString);

                foreach (KeyValuePair<string, JSONNode> kvp in jsonNode.AsObject)
                {
                    T value = default; // Initialize the value to default value of T

                    // Parse the JSONNode value to the desired type (T)
                    if (typeof(T) == typeof(float))
                    {
                        value = (T)(object)kvp.Value.AsFloat;
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        value = (T)(object)kvp.Value.AsInt;
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        value = (T)(object)kvp.Value.AsBool;
                    }
                    else
                    {
                        Debug.LogError("Unsupported type: " + typeof(T).FullName);
                    }

                    dictionary.Add(kvp.Key, value);
                }

                return dictionary;
            }

            
            
            // it checks if the game is running in the browser or in the editor
            public static bool IsRunningInBrowser()
            {
                #if UNITY_WEBGL && !UNITY_EDITOR
                        return true;
                #else
                            return false;
                #endif
            }
            
            private static void MockSetState(string key, object value)
            {
                if (MockDictionary.ContainsKey(key))
                {
                    MockDictionary[key] = value;
                }
                else
                {
                    MockDictionary.Add(key, value);
                }
            }

            private static T MockGetState<T>(string key)
            {
                if (MockDictionary.TryGetValue(key, out object value) && value is T typedValue)
                {
                    return typedValue;
                }
                else
                {
                    Debug.LogError($"No {key} in States or value is not of type {typeof(T)}");
                    return default;
                }
            }

            
            
            
            // Player class
            public class Player
            {
                
                [System.Serializable]
                public class ColorData
                {
                    public int r;
                    public int g;
                    public int b;
                    public string hexString;
                    public int hex;
                }

                [System.Serializable]
                public class Profile
                {
                    public ColorData color;
                    public string name;
                    public string photo;
                }
                
                
                public string id;
                private static int totalObjects = 0;

               
                public Player(string id)
                {
                    this.id = id;
                    totalObjects++;

                    if (IsRunningInBrowser())
                    {
                        OnQuitCallbacks.Add(OnQuitDefaultCallback);
                        OnQuitInternal(this.id, OnQuitWrapperCallback);
                    }
                    else
                    {
                        Debug.Log("Test Player");
                    }
                }
                
                [DllImport("__Internal")]
                private static extern void OnQuitInternal(string id, Action callback);

                private static List<Action> OnQuitCallbacks = new List<Action>();
                
                
                private void OnQuitDefaultCallback()
                {
                    Players.Remove(id);
                }
                
                [MonoPInvokeCallback(typeof(Action))]
                private static void OnQuitWrapperCallback()
                {
                    if (OnQuitCallbacks != null)
                    {
                        foreach (var callback in OnQuitCallbacks)
                        {
                            callback?.Invoke();
                        }
                    }
                }

                public void OnQuit(Action callback)
                {
                    OnQuitCallbacks.Add(callback);
                }

                
                public void SetState(string key, int value, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetPlayerStateByPlayerId(id, key, value, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(key, value);
                    }
                }

                

                public void SetState(string key, float value, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetPlayerStateByPlayerId(id, key, value, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(key, value);
                    }
                }

                public void SetState(string key, bool value, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetPlayerStateByPlayerId(id, key, value, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(key, value);
                    }
                }

                public void SetState(string key, string value, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetPlayerStateStringById(id, key, value, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(key, value);
                    }
                }

                public int GetStateInt(string key)
                {
                    if (IsRunningInBrowser())
                    {
                        return GetPlayerStateIntById(id, key);
                    }
                    else
                    {
                        return MockGetState<int>(key);
                    }
                }

                public float GetStateFloat(string key)
                {
                    if (IsRunningInBrowser())
                    {
                        return GetPlayerStateFloatById(id, key);
                    }
                    else
                    {
                        return MockGetState<float>(key);
                    }
                }

                public string GetStateString(string key)
                {
                    if (IsRunningInBrowser())
                    {
                        return GetPlayerStateStringById(id, key);
                    }
                    else
                    {
                        return MockGetState<string>(key);
                    }
                }

                public bool GetStateBool(string key)
                {
                    if (IsRunningInBrowser())
                    {
                        if (GetPlayerStateIntById(id, key) == 1)
                        {
                            return true;
                        }
                        else if (GetPlayerStateIntById(id, key) == 0)
                        {
                            return false;
                        }
                        else
                        {
                            Debug.LogError("GetPlayerStateByPlayerId: " + key + " is not a bool");
                            return false;
                        }
                    }
                    else
                    {
                        return MockGetState<bool>(key);
                    }

                }

                public void SetState(string key, Dictionary<string, int> values, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetStateHelper(id, key, values, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
                    }
                }

                public void SetState(string key, Dictionary<string, float> values, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetStateHelper(id, key, values, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
                    }
                }

                public void SetState(string key, Dictionary<string, bool> values, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetStateHelper(id, key, values, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
                    }
                }

                public void SetState(string key, Dictionary<string, string> values, bool reliable = false)
                {
                    if (IsRunningInBrowser())
                    {
                        SetStateHelper(id, key, values, reliable);
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
                    }
                }

                public Dictionary<string, float> GetStateFloat(string id, string key)
                {
                    string jsonString = GetPlayerStateDictionary(id, key);
                    return ParseJsonToDictionary<float>(jsonString);
                }


                [DllImport("__Internal")]
                private static extern void SetPlayerStateByPlayerId(string playerID, string key, int value, bool reliable = false);

                [DllImport("__Internal")]
                private static extern void SetPlayerStateByPlayerId(string playerID, string key, float value, bool reliable = false);

                [DllImport("__Internal")]
                private static extern void SetPlayerStateByPlayerId(string playerID, string key, bool value, bool reliable = false);

                [DllImport("__Internal")]
                private static extern void SetPlayerStateStringById(string playerID, string key, string value, bool reliable = false);

                [DllImport("__Internal")]
                private static extern string GetProfileByPlayerId(string playerID);  

                public Profile GetProfile()
                {
                    if (IsRunningInBrowser())
                    {
                        string jsonString = GetProfileByPlayerId(id);
                        Profile profileData = JsonUtility.FromJson<Profile>(jsonString);
                        return profileData;
                    }
                    else
                    {
                        ColorData testColor = new ColorData()
                        {
                            r = 166,
                            g = 0,
                            b = 142,
                            hexString = "#a6008e",
                        };


                        Profile testProfile = new Profile()
                        {
                            name = "CoolPlayTest",
                            color = testColor,
                            photo = "testPhoto",
                        };
                        return testProfile;

                    }
                }
                
                
                [DllImport("__Internal")]
                private static extern int GetPlayerStateIntById(string playerID, string key);

                [DllImport("__Internal")]
                private static extern float GetPlayerStateFloatById(string playerID, string key);

                [DllImport("__Internal")]
                private static extern string GetPlayerStateStringById(string playerID, string key);

                // Helpers
                [DllImport("__Internal")]
                private static extern void SetPlayerStateDictionary(string playerID, string key, string jsonValues, bool reliable = false);

                [DllImport("__Internal")]
                private static extern string GetPlayerStateDictionary(string playerID, string key);

                private void SetStateHelper<T>(string id, string key, Dictionary<string, T> values, bool reliable = false)
                {
                    JSONObject jsonObject = new JSONObject();

                    // Add key-value pairs to the JSON object
                    foreach (var kvp in values)
                    {
                        // Convert the value to double before adding to JSONNode
                        double value = Convert.ToDouble(kvp.Value);
                        jsonObject.Add(kvp.Key, value);
                    }

                    // Serialize the JSON object to a string
                    string jsonString = jsonObject.ToString();

                    // Output the JSON string
                    SetPlayerStateDictionary(id, key, jsonString, reliable);
                }
            }

        }
        
    }
    
