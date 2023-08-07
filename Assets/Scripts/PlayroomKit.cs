    using UnityEngine;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using AOT;
    using System;
    using SimpleJSON;
    
    namespace Playroom
    {
        public class PlayroomKit
        {

            public static Dictionary<string, Player> Players = new Dictionary<string, Player>();

            private static Action InsertCoinCallback = null;
            
            [DllImport("__Internal")]
            private static extern void InsertCoinInternal(Action callback);
            
            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeInsertCoin()
            {
                InsertCoinCallback?.Invoke();
            }

            public static void InsertCoin(Action callback)
            {
                InsertCoinCallback = callback;
                InsertCoinInternal(InvokeInsertCoin);
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
                onPlayerJoinCallback = playerCallback;
                OnPlayerJoinInternal(OnPlayerJoinWrapperCallback);
            }  
            
            public static Dictionary<string, Player> GetPlayers()
            {
                return Players;
            }
            
            public static Player GetPlayer(string playerId)
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

            [DllImport("__Internal")]
            public static extern bool IsHost();

            [DllImport("__Internal")]
            public static extern bool IsStreamMode();

            [DllImport("__Internal")]
            private static extern string MyPlayerInternal();
            
            public static Player MyPlayer()
            {
                return new Player(MyPlayerInternal());
            }

            [DllImport("__Internal")]
            private static extern void SetStateString(string key, string value);

            [DllImport("__Internal")]
            public static extern void SetState(string key, int value);

            [DllImport("__Internal")]
            public static extern void SetState(string key, float value);

            [DllImport("__Internal")]
            public static extern void SetState(string key, bool value);

            public static void SetState(string key, string value)
            {
                SetStateString(key, value);
            }

            [DllImport("__Internal")]
            private static extern void SetStateDictionary(string key, string jsonValues);


            public static void SetState(string key, Dictionary<string, int> values)
            {
                SetStateHelper(key, values);
            }

            public static void SetState(string key, Dictionary<string, float> values)
            {
                SetStateHelper(key, values);
            }

            public static void SetState(string key, Dictionary<string, bool> values)
            {
                SetStateHelper(key, values);
            }

            public static void SetState(string key, Dictionary<string, string> values)
            {
                SetStateHelper(key, values);
            }


            // GETTERS
            [DllImport("__Internal")]
            public static extern string GetStateString(string key);

            [DllImport("__Internal")]
            public static extern int GetStateInt(string key);

            [DllImport("__Internal")]
            public static extern float GetStateFloat(string key);

            public static bool GetStateBool(string key)
            {
                if (GetStateInt(key) == 1)
                {
                    return true;
                }
                else if (GetStateInt(key) == 0)
                {
                    return false;
                }
                else
                {
                    Debug.LogError("GetStateBool: " + key + " is not a bool");
                    return false;
                }

            }

            [DllImport("__Internal")]
            private static extern string GetStateDictionary(string key);

            public static Dictionary<string, float> GetStateFloatDict(string key)
            {
                string jsonString = GetStateDictionary(key);
                return ParseJsonToDictionary<float>(jsonString);
            }

            // helper functions:
            private static void SetStateHelper<T>(string key, Dictionary<string, T> values)
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
                SetStateDictionary(key, jsonString);
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

                    OnQuitCallbacks.Add(OnQuitDefaultCallback) ;
                    OnQuitInternal(this.id, OnQuitWrapperCallback);
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

                
                public void SetState(string key, int value)
                {
                    SetPlayerStateByPlayerId(id, key, value);
                }

                public void SetState(string key, float value)
                {
                    SetPlayerStateByPlayerId(id, key, value);
                }

                public void SetState(string key, bool value)
                {
                    SetPlayerStateByPlayerId(id, key, value);
                }

                public void SetState(string key, string value)
                {
                    SetPlayerStateStringById(id, key, value);
                }

                public int GetStateInt(string key)
                {
                    return GetPlayerStateIntById(id, key);
                }

                public float GetStateFloat(string key)
                {
                    return GetPlayerStateFloatById(id, key);
                }

                public string GetStateString(string key)
                {
                    return GetPlayerStateStringById(id, key);
                }

                public bool GetStateBool(string key)
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

                public void SetState(string key, Dictionary<string, int> values)
                {
                    SetStateHelper(id, key, values);
                }

                public void SetState(string key, Dictionary<string, float> values)
                {
                    SetStateHelper(id, key, values);
                }

                public void SetState(string key, Dictionary<string, bool> values)
                {
                    SetStateHelper(id, key, values);
                }

                public void SetState(string key, Dictionary<string, string> values)
                {
                    SetStateHelper(id, key, values);
                }

                public Dictionary<string, float> GetStateFloat(string id, string key)
                {
                    string jsonString = GetPlayerStateDictionary(id, key);
                    return ParseJsonToDictionary<float>(jsonString);
                }


                [DllImport("__Internal")]
                private static extern void SetPlayerStateByPlayerId(string playerID, string key, int value);

                [DllImport("__Internal")]
                private static extern void SetPlayerStateByPlayerId(string playerID, string key, float value);

                [DllImport("__Internal")]
                private static extern void SetPlayerStateByPlayerId(string playerID, string key, bool value);

                [DllImport("__Internal")]
                private static extern void SetPlayerStateStringById(string playerID, string key, string value);

                [DllImport("__Internal")]
                private static extern string GetProfileByPlayerId(string playerID);  

                public Profile GetProfile()
                {
                    string jsonString = GetProfileByPlayerId(id);
                    Profile profileData = JsonUtility.FromJson<Profile>(jsonString);
                    return profileData;
                }
                
                
                [DllImport("__Internal")]
                private static extern int GetPlayerStateIntById(string playerID, string key);

                [DllImport("__Internal")]
                private static extern float GetPlayerStateFloatById(string playerID, string key);

                [DllImport("__Internal")]
                private static extern string GetPlayerStateStringById(string playerID, string key);

                // Helpers
                [DllImport("__Internal")]
                private static extern void SetPlayerStateDictionary(string playerID, string key, string jsonValues);

                [DllImport("__Internal")]
                private static extern string GetPlayerStateDictionary(string playerID, string key);

                private void SetStateHelper<T>(string id, string key, Dictionary<string, T> values)
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
                    SetPlayerStateDictionary(id, key, jsonString);
                }

                private static Dictionary<string, object> JsonNodeToDictionary(string jsonString)
                {
                    JSONNode jsonNode = JSON.Parse(jsonString);
                    Dictionary<string, object> dict = new Dictionary<string, object>();

                    foreach (KeyValuePair<string, JSONNode> kvp in jsonNode.AsObject)
                    {
                        if (kvp.Value.IsObject)
                        {
                            dict[kvp.Key] = JsonNodeToDictionary(kvp.Value.Value); 
                        }
                        else if (kvp.Value.IsArray)
                        {
                            List<object> list = new List<object>();
                            foreach (JSONNode childNode in kvp.Value.AsArray)
                            {
                                if (childNode.IsObject)
                                {
                                    list.Add(JsonNodeToDictionary(childNode.Value)); // Pass childNode.Value
                                }
                                else
                                {
                                    list.Add(childNode.Value);
                                }
                            }
                            dict[kvp.Key] = list;
                        }
                        else
                        {
                            dict[kvp.Key] = kvp.Value.Value;
                        }
                    }

                    return dict;
                }
                
                
            }

        }

    }