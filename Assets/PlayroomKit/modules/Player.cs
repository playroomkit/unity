using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using AOT;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    // Player class
    public partial class PlayroomKit
    {
        public interface IPlayerInteraction
        {
            void InvokeOnQuitWrapperCallback();
        }

        public class Player : IPlayerInteraction
        {
            [Serializable]
            public class Profile
            {
                [NonSerialized] public UnityEngine.Color color;

                public JsonColor jsonColor;
                public string name;
                public string photo;

                [Serializable]
                public class JsonColor
                {
                    public int r;
                    public int g;
                    public int b;
                    public string hexString;
                    public int hex;
                }
            }


            public string id;
            private static int totalObjects = 0;


            public Player(string id)
            {
                this.id = id;
                totalObjects++;

                if (IsRunningInBrowser())
                {
                    // OnQuitCallbacks.Add(OnQuitDefaultCallback);
                }
                else
                {
                    if (!isPlayRoomInitialized) ///
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    else
                        Debug.Log("Mock Player Created");
                }
            }

            private List<Action<string>> OnQuitCallbacks = new();


            private void OnQuitDefaultCallback()
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("Playroom not initialized yet! Please call InsertCoin.");
                }

                Players.Remove(id);
            }

            [MonoPInvokeCallback(typeof(Action))]
            private void OnQuitWrapperCallback()
            {
                if (OnQuitCallbacks != null)
                    foreach (var callback in OnQuitCallbacks)
                        callback?.Invoke(id);
            }

            void IPlayerInteraction.InvokeOnQuitWrapperCallback()
            {
                OnQuitWrapperCallback();
            }

            public Action OnQuit(Action<string> callback)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                    return null;
                }
                else
                {
                    OnQuitCallbacks.Add(callback);

                    void Unsubscribe()
                    {
                        OnQuitCallbacks.Remove(callback);
                    }

                    return Unsubscribe;
                }
            }

            public void SetState(string key, int value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetPlayerStateByPlayerId(id, key, value, reliable);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(id, key, value);
                    }
                }
            }


            public void SetState(string key, float value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetPlayerStateFloatByPlayerId(id, key, value.ToString(CultureInfo.InvariantCulture), reliable);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        MockSetState(id, key, value);
                    }
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
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(id, key, value);
                    }
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
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(id, key, value);
                    }
                }
            }

            public void SetState(string key, object value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    string jsonString = JsonUtility.ToJson(value);
                    // Debug.Log(jsonString);
                    SetPlayerStateStringById(id, key, jsonString, reliable);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        MockSetState(id, key, value);
                    }
                }
            }

            public T GetState<T>(string key)
            {
                if (IsRunningInBrowser())
                {
                    Type type = typeof(T);
                    if (type == typeof(int)) return (T)(object)GetPlayerStateIntById(id, key);
                    else if (type == typeof(float)) return (T)(object)GetPlayerStateFloatById(id, key);
                    else if (type == typeof(bool)) return (T)(object)GetPlayerStateBoolById(id, key);
                    else if (type == typeof(string)) return (T)(object)GetPlayerStateStringById(id, key);
                    else if (type == typeof(Vector3))
                    {
                        string json = GetPlayerStateStringById(id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Vector3>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else if (type == typeof(Color))
                    {
                        string json = GetPlayerStateStringById(id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Color>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else if (type == typeof(Vector2))
                    {
                        string json = GetPlayerStateStringById(id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Vector2>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else if (type == typeof(Quaternion))
                    {
                        string json = GetPlayerStateStringById(id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Quaternion>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else throw new NotSupportedException($"Type {typeof(T)} is not supported by GetState");
                }

                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return default;
                }

                return MockGetState<T>(id, key);
            }

            public Dictionary<string, T> GetState<T>(string key, bool isReturnDictionary = false)
            {
                if (IsRunningInBrowser() && isReturnDictionary)
                {
                    var jsonString = GetPlayerStateDictionary(id, key);
                    return ParseJsonToDictionary<T>(jsonString);
                }
                else
                {
                    if (isPlayRoomInitialized)
                    {
                        if (isReturnDictionary)
                        {
                            return MockGetState<Dictionary<string, T>>(key);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return default;
                    }
                }
            }

            private int GetPlayerStateInt(string key)
            {
                if (IsRunningInBrowser())
                {
                    return GetPlayerStateIntById(id, key);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return default;
                    }
                    else
                    {
                        return MockGetState<int>(key);
                    }
                }
            }

            private float GetPlayerStateFloat(string key)
            {
                if (IsRunningInBrowser())
                {
                    return GetPlayerStateFloatById(id, key);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return default;
                    }
                    else
                    {
                        return MockGetState<float>(key);
                    }
                }
            }

            private string GetPlayerStateString(string key)
            {
                if (IsRunningInBrowser())
                {
                    return GetPlayerStateStringById(id, key);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return default;
                    }
                    else
                    {
                        return MockGetState<string>(key);
                    }
                }
            }

            private bool GetPlayerStateBool(string key)
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
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return default;
                    }
                    else
                    {
                        return MockGetState<bool>(key);
                    }
                }
            }

            // Dictionaries:
            public void SetState(string key, Dictionary<string, int> values, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateHelper(id, key, values, reliable);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        MockSetState(id, key, values);
                    }
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
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        MockSetState(id, key, values);
                    }
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
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        MockSetState(id, key, values);
                    }
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
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        MockSetState(id, key, values);
                    }
                }
            }

            public void WaitForState(string StateKey, Action onStateSetCallback = null)
            {
                if (IsRunningInBrowser())
                {
                    WaitForPlayerStateInternal(id, StateKey, onStateSetCallback);
                }
            }

            public void Kick(Action OnKickCallBack = null)
            {
                if (IsRunningInBrowser())
                {
                    OnKickCallBack = onKickCallBack;
                    KickInternal(id, InvokeKickCallBack);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return;
                    }
                    else
                    {
                        var player = GetPlayer(PlayerId);
                        Players.Remove(player.id);
                        onKickCallBack?.Invoke();
                    }
                }
            }


            private static Profile ParseProfile(string json)
            {
                var jsonNode = JSON.Parse(json);
                var profileData = new Profile();
                profileData.jsonColor = new Profile.JsonColor
                {
                    r = jsonNode["color"]["r"].AsInt,
                    g = jsonNode["color"]["g"].AsInt,
                    b = jsonNode["color"]["b"].AsInt,
                    hexString = jsonNode["color"]["hexString"].Value,
                    hex = jsonNode["color"]["hex"].AsInt
                };

                ColorUtility.TryParseHtmlString(profileData.jsonColor.hexString, out UnityEngine.Color color1);
                profileData.color = color1;
                profileData.name = jsonNode["name"].Value;
                profileData.photo = jsonNode["photo"].Value;

                return profileData;
            }

            public Profile GetProfile()
            {
                if (IsRunningInBrowser())
                {
                    var jsonString = GetProfileByPlayerId(id);
                    var profileData = ParseProfile(jsonString);
                    return profileData;
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return default;
                    }
                    else
                    {
                        Profile.JsonColor mockJsonColor = new()
                        {
                            r = 166,
                            g = 0,
                            b = 142,
                            hexString = "#a6008e"
                        };
                        ColorUtility.TryParseHtmlString(mockJsonColor.hexString, out UnityEngine.Color color1);
                        var testProfile = new Profile()
                        {
                            color = color1,
                            name = "MockPlayer",
                            jsonColor = mockJsonColor,
                            photo = "testPhoto"
                        };
                        return testProfile;
                    }
                }
            }


            private static Action onKickCallBack = null;

            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeKickCallBack()
            {
                onKickCallBack?.Invoke();
            }


            private static bool GetPlayerStateBoolById(string id, string key)
            {
                if (IsRunningInBrowser())
                {
                    var stateValue = GetPlayerStateIntById(id, key);
                    return stateValue == 1 ? true :
                        stateValue == 0 ? false :
                        throw new InvalidOperationException($"GetStateBool: {key} is not a bool");
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                        return false;
                    }
                    else
                    {
                        return MockGetState<bool>(key);
                    }
                }
            }


            private void SetStateHelper<T>(string id, string key, Dictionary<string, T> values, bool reliable = false)
            {
                var jsonObject = new JSONObject();

                // Add key-value pairs to the JSON object
                foreach (var kvp in values)
                {
                    // Convert the value to double before adding to JSONNode
                    var value = Convert.ToDouble(kvp.Value);
                    jsonObject.Add(kvp.Key, value);
                }

                // Serialize the JSON object to a string
                var jsonString = jsonObject.ToString();

                // Output the JSON string
                SetPlayerStateDictionary(id, key, jsonString, reliable);
            }

            [DllImport("__Internal")]
            private static extern void KickInternal(string playerID, Action onKickCallBack = null);

            [DllImport("__Internal")]
            private static extern void WaitForPlayerStateInternal(string playerID, string stateKey,
                Action onStateSetCallback = null);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateByPlayerId(string playerID, string key, int value,
                bool reliable = false);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateFloatByPlayerId(string playerID, string key, string value,
                bool reliable = false);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateByPlayerId(string playerID, string key, bool value,
                bool reliable = false);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateDictionary(string playerID, string key, string jsonValues,
                bool reliable = false);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateStringById(string playerID, string key, string value,
                bool reliable = false);

            [DllImport("__Internal")]
            private static extern int GetPlayerStateIntById(string playerID, string key);

            [DllImport("__Internal")]
            private static extern float GetPlayerStateFloatById(string playerID, string key);

            [DllImport("__Internal")]
            private static extern string GetPlayerStateStringById(string playerID, string key);


            [DllImport("__Internal")]
            private static extern string GetPlayerStateDictionary(string playerID, string key);

            [DllImport("__Internal")]
            private static extern string GetProfileByPlayerId(string playerID);
        }
    }
}