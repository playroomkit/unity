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
    public partial class PlayroomKit
    {
        private static bool isPlayRoomInitialized;

        private static readonly Dictionary<string, Player> Players = new();

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

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void InvokeInsertCoin(string key)
        {
            CallbackManager.InvokeCallback(key);


#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;
#endif

        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void InvokeOnErrorInsertCoin(string error)
        {
            onError?.Invoke(error);
            Debug.LogException(new Exception(error));
        }

        private static Action<string> onError;

        public static void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null, Action onDisconnectCallback = null)
        {
            if (IsRunningInBrowser())
            {
                isPlayRoomInitialized = true;


                string onLaunchCallBackKey = CallbackManager.RegisterCallback(onLaunchCallBack, "onLaunchCallBack");
                string onDisconnectCallBackKey = CallbackManager.RegisterCallback(onDisconnectCallback, "onDisconnectCallBack");

                Debug.Log(onLaunchCallBackKey);

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

                InsertCoinInternal(
                    optionsJson, InvokeInsertCoin, __OnQuitInternalHandler, onDisconnectCallbackHandler,
                    InvokeOnErrorInsertCoin, onLaunchCallBackKey, onDisconnectCallBackKey);
            }
            else
            {
                MockInsertCoin(options, onLaunchCallBack);
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



        private static List<Action<Player>> OnPlayerJoinCallbacks = new();


        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void __OnPlayerJoinCallbackHandler(string id)
        {
            OnPlayerJoinWrapperCallback(id);
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void onDisconnectCallbackHandler(string key)
        {
            CallbackManager.InvokeCallback(key);
        }


        private static void OnPlayerJoinWrapperCallback(string id)
        {
            var player = GetPlayer(id);
            foreach (var callback in OnPlayerJoinCallbacks)
            {
                callback?.Invoke(player);
            }
        }

        public static Action OnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return null;
            }
            else
            {
                if (IsRunningInBrowser())
                {
                    if (!OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback))
                    {
                        OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
                    }
                    var CallbackID = OnPlayerJoinInternal(__OnPlayerJoinCallbackHandler);

                    void Unsubscribe()
                    {
                        OnPlayerJoinCallbacks.Remove(onPlayerJoinCallback);
                        UnsubscribeOnPlayerJoin(CallbackID);
                    }

                    return Unsubscribe;
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
                        OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
                        __OnPlayerJoinCallbackHandler(PlayerId);
                    }
                    return null;
                }
            }
        }

        private static void UnsubscribeOnPlayerJoin(string CallbackID)
        {
            UnsubscribeOnPlayerJoinInternal(CallbackID);
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




        public static void OnDisconnect(Action callback)
        {
            CallbackManager.RegisterCallback(callback);
            OnDisconnectInternal(onDisconnectCallbackHandler);
        }


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
                    MockSetState(key, value);
                }
            }
        }




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
                    MockSetState(key, values);
                }
            }
        }


        // GETTERS


        private static string GetStateString(string key)
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



        private static int GetStateInt(string key)
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



        private static float GetStateFloat(string key)
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

        private static bool GetStateBool(string key)
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
                Type type = typeof(T);
                if (type == typeof(int)) return (T)(object)GetStateInt(key);
                else if (type == typeof(float)) return (T)(object)GetStateFloat(key);
                else if (type == typeof(bool)) return (T)(object)GetStateBool(key);
                else if (type == typeof(string)) return (T)(object)GetStateString(key);
                else if (type == typeof(Vector2)) return JsonUtility.FromJson<T>(GetStateString(key));
                else if (type == typeof(Vector3)) return JsonUtility.FromJson<T>(GetStateString(key));
                else if (type == typeof(Vector4)) return JsonUtility.FromJson<T>(GetStateString(key));
                else if (type == typeof(Quaternion)) return JsonUtility.FromJson<T>(GetStateString(key));
                else
                {
                    Debug.LogError($"GetState<{type}> is not supported.");
                    return default;
                }
            }
            else
            {
                if (isPlayRoomInitialized)
                {
                    return MockGetState<T>(key);
                }
                else
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return default;
                }
            }
        }


        public static Dictionary<string, T> GetState<T>(string key, bool isReturnDictionary = false)
        {
            if (IsRunningInBrowser() && isReturnDictionary)
            {
                var jsonString = GetStateDictionaryInternal(key);
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




        [MonoPInvokeCallback(typeof(Action<string, string>))]
        private static void InvokeCallback(string stateKey, string stateVal)
        {
            CallbackManager.InvokeCallback(stateKey, stateVal);
        }

        public static void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            if (IsRunningInBrowser())
            {
                CallbackManager.RegisterCallback(onStateSetCallback, stateKey);

                WaitForStateInternal(stateKey, InvokeCallback);
            }
        }





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



        private static void UnsubscribeOnQuit()
        {
            UnsubscribeOnQuitInternal();
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void __OnQuitInternalHandler(string playerId)
        {
            if (Players.TryGetValue(playerId, out Player player))
            {
                ((IPlayerInteraction)player).InvokeOnQuitWrapperCallback();
            }
            else
            {
                Debug.LogError("[__OnQuitInternalHandler] Couldn't find player with id " + playerId);
            }
        }


        // Joystick
        public static void CreateJoyStick(JoystickOptions options)
        {
            var jsonStr = ConvertJoystickOptionsToJson(options);
            CreateJoystickInternal(jsonStr);
        }

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

        [Serializable]
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


        [Serializable]
        public class Dpad
        {
            public string x;
            public string y;
        }

        static Action startMatchmakingCallback = null;
        public static void StartMatchmaking(Action callback = null)
        {
            if (IsRunningInBrowser())
            {
                startMatchmakingCallback = callback;
                StartMatchmakingInternal(InvokeStartMatchmakingCallback);
            }
            else
            {
                Debug.LogError("[Mock Mode] Matchmaking is not supported in Mock Mode! yet.\nPlease build the project to test Matchmaking.");
            }
        }

        [MonoPInvokeCallback(typeof(Action))]
        private static void InvokeStartMatchmakingCallback()
        {
            startMatchmakingCallback?.Invoke();
        }


    }


}
