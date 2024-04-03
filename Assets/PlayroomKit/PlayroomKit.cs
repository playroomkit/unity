using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using AOT;
using System;
using SimpleJSON;
using System.Collections;

namespace Playroom
{
    public class PlayroomKit
    {
        private static bool isPlayRoomInitialized;


        /// <summary>
        /// Required Mock Mode:
        /// </summary>
        private const string PlayerId = "mockPlayer";
        private static bool mockIsStreamMode;

        /* 
        This is private, instead of public, to prevent tampering in Mock Mode.
        Reason: In Mock Mode, only a single player can be tested. 
        Ref: https://docs.joinplayroom.com/usage/unity#mock-mode
        */
        private static Dictionary<string, object> MockDictionary = new();

        public static readonly Dictionary<string, Player> Players = new();


        [Serializable]
        public class InitOptions
        {
            public bool streamMode = false;
            public bool allowGamepads = false;
            public string baseUrl = "";
            public string[] avatars = null;
            public string roomCode = "";
            public bool skipLobby = false;
            public int reconnectGracePeriod = 0;
            public int? maxPlayersPerRoom;
            public string? gameId;
            public bool discord = false;

            public Dictionary<string, object> defaultStates = null;
            public Dictionary<string, object> defaultPlayerStates = null;

            public bool matchmaking = false;

        }

        public class MatchMakingOptions
        {
            public int waitBeforeCreatingNewRoom = 5000;
        }

        private static Action InsertCoinCallback = null;
        private static Action OnDisconnectCallback = null;

        [DllImport("__Internal")]
        private static extern void InsertCoinInternal(string options, Action onLaunchCallback, Action<string> onQuitInternalCallback, Action onDisconnectCallback);

        [MonoPInvokeCallback(typeof(Action))]
        private static void InvokeInsertCoin()
        {
            InsertCoinCallback?.Invoke();
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;
#endif

        }

        // optional InitOptions
        public static void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null, Action onDisconnectCallback = null)
        {
            if (IsRunningInBrowser())
            {
                isPlayRoomInitialized = true;
                InsertCoinCallback = onLaunchCallBack;
                OnDisconnectCallback = onDisconnectCallback;
                string optionsJson = null;
                if (options != null)
                {
                    optionsJson = SerializeInitOptions(options);
                }

                if (options.skipLobby == false)
                {

#if UNITY_WEBGL && !UNITY_EDITOR
                        WebGLInput.captureAllKeyboardInput = false;
#endif
                }

                InsertCoinInternal(optionsJson, InvokeInsertCoin, __OnQuitInternalHandler, onDisconnectCallbackHandler);
            }
            else
            {
                isPlayRoomInitialized = true;

                Debug.Log("Coin Inserted");

                string optionsJson = null;
                if (options != null) optionsJson = SerializeInitOptions(options);
                onLaunchCallBack?.Invoke();
            }
        }

        private static string SerializeInitOptions(InitOptions options)
        {
            if (options == null) return null;

            JSONNode node = JSON.Parse("{}");

            node["streamMode"] = options.streamMode;
            node["allowGamepads"] = options.allowGamepads;
            node["baseUrl"] = options.baseUrl;

            if (options.avatars != null)
            {
                JSONArray avatarsArray = new JSONArray();
                foreach (string avatar in options.avatars)
                {
                    avatarsArray.Add(avatar);
                }
                node["avatars"] = avatarsArray;
            }

            node["roomCode"] = options.roomCode;
            node["skipLobby"] = options.skipLobby;
            node["reconnectGracePeriod"] = options.reconnectGracePeriod;

            node["matchmaking"] = options.matchmaking;

            if (options.maxPlayersPerRoom.HasValue)
            {
                node["maxPlayersPerRoom"] = options.maxPlayersPerRoom.Value;
            }

            if (options.gameId != null)
            {
                node["gameId"] = options.gameId;
            }

            node["discord"] = options.discord;

            if (options.defaultStates != null)
            {
                JSONObject defaultStatesObject = new JSONObject();
                foreach (var kvp in options.defaultStates)
                {
                    defaultStatesObject[kvp.Key] = ConvertValueToJSON(kvp.Value);
                }
                node["defaultStates"] = defaultStatesObject;
            }

            if (options.defaultPlayerStates != null)
            {
                JSONObject defaultPlayerStatesObject = new JSONObject();
                foreach (var kvp in options.defaultPlayerStates)
                {
                    defaultPlayerStatesObject[kvp.Key] = ConvertValueToJSON(kvp.Value);
                }
                node["defaultPlayerStates"] = defaultPlayerStatesObject;
            }


            return node.ToString();
        }

        private static JSONNode ConvertValueToJSON(object value)
        {
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is int intValue)
            {
                return intValue;
            }
            else if (value is float floatValue)
            {
                return floatValue;
            }
            else if (value is bool boolValue)
            {
                return boolValue;
            }
            else
            {
                // Handle other types if needed
                return JSON.Parse("{}");
            }
        }

        [DllImport("__Internal")]
        private static extern void OnPlayerJoinInternal(Action<string> callback);

        // private static Action<Player> onPlayerJoinCallback = null;
        private static List<Action<Player>> OnPlayerJoinCallbacks = new();


        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void __OnPlayerJoinCallbackHandler(string id)
        {
            OnPlayerJoinWrapperCallback(id);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void onDisconnectCallbackHandler()
        {
            OnDisconnectCallback?.Invoke();
        }


        private static void OnPlayerJoinWrapperCallback(string id)
        {
            var player = GetPlayer(id);
            foreach (var callback in OnPlayerJoinCallbacks)
            {
                callback?.Invoke(player);
            }
            // onPlayerJoinCallback?.Invoke(player);
        }




        public static void OnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
            }
            else
            {
                if (IsRunningInBrowser())
                {
                    // onPlayerJoinCallback = playerCallback;
                    OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
                    OnPlayerJoinInternal(__OnPlayerJoinCallbackHandler);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        Debug.Log("On Player Join");
                        var testPlayer = GetPlayer(PlayerId);
                        onPlayerJoinCallback?.Invoke(testPlayer);
                    }
                }
            }
        }

        public static Dictionary<string, Player> GetPlayers()
        {
            if (!isPlayRoomInitialized)
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");

            return Players;
        }

        public static Player GetPlayer(string playerId)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError(IsRunningInBrowser()
                    ? "PlayroomKit is not loaded!. Please make sure to call InsertCoin first."
                    : "[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return default;
            }

            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }
            else
            {
                player = new Player(playerId);
                Players.Add(playerId, player);
                return player;
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [DllImport("__Internal")]
        private static extern bool IsStreamModeInternal();

        public static bool IsStreamMode()
        {
            if (IsRunningInBrowser())
            {
                return IsStreamModeInternal();
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
                    return mockIsStreamMode;
                }
            }
        }

        [DllImport("__Internal")]
        private static extern string MyPlayerInternal();

        public static Player MyPlayer()
        {
            if (IsRunningInBrowser())
            {
                var id = MyPlayerInternal();
                return GetPlayer(id);
            }
            else
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return null;
                }
                else
                {
                    return GetPlayer(PlayerId);
                }
            }
        }

        public static Player Me()
        {
            return MyPlayer();
        }

        [DllImport("__Internal")]
        public static extern string GetRoomCode();


        [DllImport("__Internal")]
        private static extern void OnDisconnectInternal(Action callback);


        public static void OnDisconnect(Action callback)
        {
            OnDisconnectCallback = callback;
            OnDisconnectInternal(onDisconnectCallbackHandler);
        }


        [DllImport("__Internal")]
        private static extern void SetStateString(string key, string value, bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetStateInternal(string key, int value, bool reliable = false);


        [DllImport("__Internal")]
        private static extern void SetStateInternal(string key, bool value, bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetStateFloatInternal(string key, string floatAsString, bool reliable = false);

        public static void SetState(string key, string value, bool reliable = false)
        {
            if (IsRunningInBrowser())
            {
                SetStateString(key, value, reliable);
            }
            else
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
            }
        }

        public static void SetState(string key, float value, bool reliable = false)
        {
            if (IsRunningInBrowser())
            {
                var floatAsString = value.ToString(CultureInfo.InvariantCulture);
                SetStateFloatInternal(key, floatAsString, reliable);
            }
            else
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {value}");
                    MockSetState(key, value);
                }
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                }
                else
                {
                    Debug.Log($"State Set! Key: {key}, Value: {values}");
                    MockSetState(key, values);
                }
            }
        }


        // GETTERS
        [DllImport("__Internal")]
        private static extern string GetStateStringInternal(string key);

        public static string GetStateString(string key)
        {
            if (IsRunningInBrowser())
            {
                return GetStateStringInternal(key);
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

        [DllImport("__Internal")]
        private static extern float GetStateFloatInternal(string key);

        public static float GetStateFloat(string key)
        {
            if (IsRunningInBrowser())
            {
                return GetStateFloatInternal(key);
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

        public static T GetState<T>(string key)
        {
            if (IsRunningInBrowser())
            {
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)GetStateInt(key);
                }
                else if (typeof(T) == typeof(float))
                {
                    return (T)(object)GetStateFloat(key);
                }
                else if (typeof(T) == typeof(bool))
                {
                    return (T)(object)GetStateBool(key);
                }
                else if (typeof(T) == typeof(string))
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
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return default;
                }
                else
                {
                    return MockGetState<T>(key);
                }
            }
        }

        [DllImport("__Internal")]
        private static extern void WaitForStateInternal(string stateKey, Action onStateSetCallback = null);


        private static Action OnStateChangeCallBack = null;

        [MonoPInvokeCallback(typeof(Action))]
        private static void InvokeCallback()
        {
            OnStateChangeCallBack?.Invoke();
        }

        public static void WaitForState(string stateKey, Action onStateSetCallback = null)
        {
            if (IsRunningInBrowser())
            {
                OnStateChangeCallBack = onStateSetCallback;
                WaitForStateInternal(stateKey, InvokeCallback);
            }
        }

        [DllImport("__Internal")]
        private static extern void WaitForPlayerStateInternal(string playerID, string StateKey, Action onStateSetCallback);

        Action Callback = null;
        public void WaitForPlayerState(string playerID, string StateKey, Action onStateSetCallback = null)
        {
            if (IsRunningInBrowser())
            {
                Callback = onStateSetCallback;
                WaitForPlayerStateInternal(playerID, StateKey, OnStateSetCallback);
            }
        }

        [MonoPInvokeCallback(typeof(Action))]
        void OnStateSetCallback()
        {
            Callback?.Invoke();
        }


        [DllImport("__Internal")]
        private static extern string GetStateDictionaryInternal(string key);

        public static Dictionary<string, T> GetStateDict<T>(string key)
        {
            if (IsRunningInBrowser())
            {
                var jsonString = GetStateDictionaryInternal(key);
                return ParseJsonToDictionary<T>(jsonString);
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
                    return MockGetState<Dictionary<string, T>>(key);
                }
            }
        }

        // Utils:
        private static void SetStateHelper<T>(string key, Dictionary<string, T> values, bool reliable = false)
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
            SetStateDictionary(key, jsonString, reliable);
        }

        private static Dictionary<string, T> ParseJsonToDictionary<T>(string jsonString)
        {
            var dictionary = new Dictionary<string, T>();
            var jsonNode = JSON.Parse(jsonString);

            foreach (var kvp in jsonNode.AsObject)
            {
                T value = default; // Initialize the value to default value of T

                // Parse the JSONNode value to the desired type (T)
                if (typeof(T) == typeof(float))
                    value = (T)(object)kvp.Value.AsFloat;
                else if (typeof(T) == typeof(int))
                    value = (T)(object)kvp.Value.AsInt;
                else if (typeof(T) == typeof(bool))
                    value = (T)(object)kvp.Value.AsBool;
                else
                    Debug.LogError("Unsupported type: " + typeof(T).FullName);

                dictionary.Add(kvp.Key, value);
            }

            return dictionary;
        }

        [DllImport("__Internal")]
        private static extern void ResetStatesInternal(string keysToExclude = null, Action OnStatesReset = null);

        private static Action onstatesReset;
        private static Action onplayersStatesReset;

        public static void ResetStates(string[] keysToExclude = null, Action OnStatesReset = null)
        {
            if (IsRunningInBrowser())
            {
                onstatesReset = OnStatesReset;
                string keysJson = keysToExclude != null ? CreateJsonArray(keysToExclude).ToString() : null;
                ResetStatesInternal(keysJson, InvokeResetCallBack);
            }
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void InvokeResetCallBack()
        {
            onstatesReset?.Invoke();
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void InvokePlayersResetCallBack()
        {
            onplayersStatesReset?.Invoke();
        }


        [DllImport("__Internal")]
        private static extern void ResetPlayersStatesInternal(string keysToExclude, Action OnPlayersStatesReset = null);

        public static void ResetPlayersStates(string[] keysToExclude = null, Action OnStatesReset = null)
        {
            if (IsRunningInBrowser())
            {
                onstatesReset = OnStatesReset;
                string keysJson = keysToExclude != null ? CreateJsonArray(keysToExclude).ToString() : null;
                ResetPlayersStatesInternal(keysJson, InvokePlayersResetCallBack);
            }
        }

        private static JSONArray CreateJsonArray(string[] array)
        {
            JSONArray jsonArray = new JSONArray();

            foreach (string item in array)
            {
                jsonArray.Add(item);
            }

            return jsonArray;
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
                MockDictionary[key] = value;
            else
                MockDictionary.Add(key, value);
        }

        private static T MockGetState<T>(string key)
        {
            if (MockDictionary.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            else
            {
                Debug.LogError($"No {key} in States or value is not of type {typeof(T)}");
                return default;
            }
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void __OnQuitInternalHandler(string playerId)
        {
            if (Players.TryGetValue(playerId, out Player player))
            {
                player.OnQuitWrapperCallback();
            }
            else
            {
                Debug.LogError("[__OnQuitInternalHandler] Couldn't find player with id " + playerId);
            }
        }


        // Joystick
        [DllImport("__Internal")]
        private static extern void CreateJoystickInternal(string joyStickOptionsJson);

        public static void CreateJoyStick(JoystickOptions options)
        {
            var jsonStr = ConvertJoystickOptionsToJson(options);
            CreateJoystickInternal(jsonStr);
        }

        [DllImport("__Internal")]
        private static extern string DpadJoystickInternal();

        public static Dpad DpadJoystick()
        {
            var jsonString = DpadJoystickInternal();
            Dpad myDpad = JsonUtility.FromJson<Dpad>(jsonString);
            return myDpad;
        }

        private static string ConvertJoystickOptionsToJson(JoystickOptions options)
        {
            JSONNode joystickOptionsJson = new JSONObject();
            joystickOptionsJson["type"] = options.type;

            // Serialize the buttons array
            JSONArray buttonsArray = new JSONArray();
            foreach (ButtonOptions button in options.buttons)
            {
                JSONObject buttonJson = new JSONObject();
                buttonJson["id"] = button.id;
                buttonJson["label"] = button.label;
                buttonJson["icon"] = button.icon;
                buttonsArray.Add(buttonJson);
            }
            joystickOptionsJson["buttons"] = buttonsArray;

            // Serialize the zones property (assuming it's not null)
            if (options.zones != null)
            {
                JSONObject zonesJson = new JSONObject();
                zonesJson["up"] = ConvertButtonOptionsToJson(options.zones.up);
                zonesJson["down"] = ConvertButtonOptionsToJson(options.zones.down);
                zonesJson["left"] = ConvertButtonOptionsToJson(options.zones.left);
                zonesJson["right"] = ConvertButtonOptionsToJson(options.zones.right);
                joystickOptionsJson["zones"] = zonesJson;
            }

            return joystickOptionsJson.ToString();
        }

        // Function to convert ButtonOptions to JSON
        private static JSONNode ConvertButtonOptionsToJson(ButtonOptions button)
        {
            JSONObject buttonJson = new JSONObject();
            buttonJson["id"] = button.id;
            buttonJson["label"] = button.label;
            buttonJson["icon"] = button.icon;
            return buttonJson;
        }


        public class JoystickOptions
        {
            public string type = "angular"; // default = angular, can be dpad

            public ButtonOptions[] buttons;
            public ZoneOptions zones = null;
        }

        [System.Serializable]
        public class ButtonOptions
        {
            public string id = null;
            public string label = "";
            public string icon = null;
        }

        public class ZoneOptions
        {
            public ButtonOptions up = null;
            public ButtonOptions down = null;
            public ButtonOptions left = null;
            public ButtonOptions right = null;
        }


        [System.Serializable]
        public class Dpad
        {
            public string x;
            public string y;
        }


        // RPC:
        public enum RpcMode
        {
            ALL,
            OTHERS,
            HOST
        }

        private static Action<string, string> RpcRegisterCallback = null;

        [DllImport("__Internal")]
        private extern static void RpcRegisterInternal(string name, Action<string, string> rpcRegisterCallback, string onResponseReturn = null);

        public static void RpcRegister(string name, Action<string, string> rpcRegisterCallback, string onResponseReturn = null)
        {
            RpcRegisterCallback = rpcRegisterCallback;
            RpcRegisterInternal(name, InvokeRpcRegisterCallBack, onResponseReturn);
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void InvokeRpcRegisterCallBack(string dataJson, string senderJson)
        {
            try
            {
                if (!Players.ContainsKey(senderJson))
                {
                    var player = new Player(senderJson);
                    Players.Add(senderJson, player);
                }
                else
                {
                    Debug.LogWarning($"Players dictionary already has a player with ID: {senderJson}!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            RpcRegisterCallback?.Invoke(dataJson, senderJson);
        }

        [DllImport("__Internal")]
        private extern static void RpcCallInternal(string name, string data, RpcMode mode, Action callbackOnResponse);

        private static Dictionary<string, List<Action>> OnResponseCallbacks = new Dictionary<string, List<Action>>();
        private static List<string> RpcEventNames = new List<string>();

        public static void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse)
        {

            string jsonData = ConvertToJson(data);

            if (OnResponseCallbacks.ContainsKey(name))
            {
                OnResponseCallbacks[name].Add(callbackOnResponse);
            }
            else
            {
                OnResponseCallbacks.Add(name, new List<Action> { callbackOnResponse });
                if (!RpcEventNames.Contains(name))
                {
                    RpcEventNames.Add(name);
                }
            }

            RpcCallInternal(name, jsonData, mode, InvokeOnResponseCallback);
        }

        // Default Mode
        public static void RpcCall(string name, object data, Action callbackOnResponse)
        {
            RpcCall(name, data, RpcMode.ALL, callbackOnResponse);
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void InvokeOnResponseCallback()
        {
            foreach (var name in RpcEventNames)
            {
                try
                {
                    if (OnResponseCallbacks.TryGetValue(name, out List<Action> callbacks))
                    {
                        foreach (var callback in callbacks)
                        {
                            callback?.Invoke();
                        }

                        RpcEventNames.Remove(name);
                        OnResponseCallbacks.Remove(name);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"C#: Error in Invoking callback for RPC event name: '{name}': {ex.Message}");
                }
            }
        }


        private static string ConvertToJson(object data)
        {
            if (data == null)
            {
                return null;
            }
            else if (data.GetType().IsPrimitive || data is string)
            {
                return data.ToString();
            }
            if (data is Vector2 vector2)
            {
                return JsonUtility.ToJson(vector2);
            }
            else if (data is Vector3 vector3)
            {
                return JsonUtility.ToJson(vector3);
            }
            else if (data is Vector4 vector4)
            {
                return JsonUtility.ToJson(vector4);
            }
            else if (data is Quaternion quaternion)
            {
                return JsonUtility.ToJson(quaternion);
            }
            else
            {
                return ConvertComplexToJson(data);
            }
        }

        private static string ConvertComplexToJson(object data)
        {
            if (data is IDictionary dictionary)
            {
                JSONObject dictNode = new JSONObject();
                foreach (DictionaryEntry entry in dictionary)
                {
                    dictNode[entry.Key.ToString()] = ConvertToJson(entry.Value);
                }
                return dictNode.ToString();
            }
            else if (data is IEnumerable enumerable)
            {
                JSONArray arrayNode = new JSONArray();
                foreach (object element in enumerable)
                {
                    arrayNode.Add(ConvertToJson(element));
                }
                return arrayNode.ToString();
            }
            else
            {
                Debug.Log($"{data} is '{data.GetType()}' which is currently not supported by RPC!");
                return JSON.Parse("{}").ToString();
            }
        }

        [DllImport("__Internal")]
        public static extern void StartMatchmaking();

        // Player class
        public class Player
        {

            [Serializable]
            public class Profile
            {
                [NonSerialized]
                public UnityEngine.Color color;

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
            public static int totalObjects = 0;


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
                    if (!isPlayRoomInitialized)
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
            public void OnQuitWrapperCallback()
            {
                if (OnQuitCallbacks != null)
                    foreach (var callback in OnQuitCallbacks)
                        callback?.Invoke(id);
            }

            public void OnQuit(Action<string> callback)
            {
                if (!isPlayRoomInitialized)
                    Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                else
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
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(key, value);
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
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {value}");
                        MockSetState(key, value);
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
                        MockSetState(key, value);
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
                        MockSetState(key, value);
                    }
                }
            }

            public void SetState(string key, object value, bool reliable = false)
            {
                if (IsRunningInBrowser())
                {
                    SetStateString(key, JsonUtility.ToJson(value), reliable);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        Debug.Log($"State Set! Key: {key}, Value: {value}");
                        MockSetState(key, value);
                    }
                }
            }

            public T GetState<T>(string key)
            {
                if (IsRunningInBrowser())
                {
                    if (typeof(T) == typeof(int))
                    {
                        return (T)(object)GetPlayerStateIntById(id, key);
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        return (T)(object)GetPlayerStateFloatById(id, key);
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        return (T)(object)GetStateBool(key);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        return (T)(object)GetPlayerStateStringById(id, key);
                    }
                    else
                    {
                        return JsonUtility.FromJson<T>(GetStateString(key));
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
                        return MockGetState<T>(key);
                    }
                }
            }

            public Dictionary<string, T> GetStateDict<T>(string key)
            {
                if (IsRunningInBrowser())
                {
                    var jsonString = GetPlayerStateDictionary(id, key);
                    return ParseJsonToDictionary<T>(jsonString);
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
                        return MockGetState<Dictionary<string, T>>(key);
                    }
                }
            }

            public int GetPlayerStateInt(string key)
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

            public float GetPlayerStateFloat(string key)
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

            public string GetPlayerStateString(string key)
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

            public bool GetPlayerStateBool(string key)
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
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
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
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
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
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
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
                        Debug.Log($"PlayerState Set! Key: {key}, Value: {values}");
                        MockSetState(key, values);
                    }
                }
            }

            public Dictionary<string, float> GetStateFloat(string id, string key)
            {
                var jsonString = GetPlayerStateDictionary(id, key);
                return ParseJsonToDictionary<float>(jsonString);
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
            }

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
            private static extern void SetPlayerStateStringById(string playerID, string key, string value,
                bool reliable = false);

            [DllImport("__Internal")]
            private static extern string GetProfileByPlayerId(string playerID);

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
                            name = "CoolPlayTest",
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

            [DllImport("__Internal")]
            private static extern void KickInternal(string playerID, Action onKickCallBack = null);

            [DllImport("__Internal")]
            private static extern void WaitForPlayerStateInternal(string playerID, string stateKey, Action onStateSetCallback = null);


            [DllImport("__Internal")]
            private static extern int GetPlayerStateIntById(string playerID, string key);

            [DllImport("__Internal")]
            private static extern float GetPlayerStateFloatById(string playerID, string key);

            [DllImport("__Internal")]
            private static extern string GetPlayerStateStringById(string playerID, string key);

            // Helpers
            [DllImport("__Internal")]
            private static extern void SetPlayerStateDictionary(string playerID, string key, string jsonValues,
                bool reliable = false);

            [DllImport("__Internal")]
            private static extern string GetPlayerStateDictionary(string playerID, string key);

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
        }
    }


}
