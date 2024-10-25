using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using AOT;
using System;
using SimpleJSON;
using Playroom;



namespace Playroom
{
    public partial class PlayroomKit
    {
        
        //DI
        private IPlayroomBase _playroomService;
        private static PlayroomKit _instance;
        private IRPC _rpc;
        public static bool isPlayRoomInitialized;
        private static readonly Dictionary<string, Player> Players = new();
        
        static Action startMatchmakingCallback = null;


        public PlayroomKit()
        {
            if (IsRunningInBrowser())
            {
                _playroomService = new PlayroomService(new PlayroomKitInterop());
                _rpc = new RPC(this);
            }
            else
            {
                _playroomService = new LocalMockPlayroomService();
                _rpc = new RPCLocal();
            }
        }
        
        public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
            Action onDisconnectCallback = null)
        {
            _playroomService.InsertCoin(options, onLaunchCallBack, onDisconnectCallback);
        }
        
        public bool IsHost()
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return false;
            }
            return _playroomService.IsHost();
        }
        
        public Action OnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return null;
            }

            return _playroomService.OnPlayerJoin(onPlayerJoinCallback);
        }
        
        public static Player GetPlayer(string playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }
            else
            {
                if (!IsRunningInBrowser())
                {
                    player = new Player(playerId, new LocalPlayerService());
                }
                else
                {
                    player = new Player(playerId, new PlayerService());
                }

                Players.Add(playerId, player);
                return player;
            }
        }
        
        public void SetState<T>(string key, T value, bool reliable = false)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            _playroomService.SetState(key, value, reliable);
        }
        
        public T GetState<T>(string key)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return default;
            }
            return _playroomService.GetState<T>(key);
        }
        
        public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
            string onResponseReturn = null)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            
            _rpc.RpcRegister(name, rpcRegisterCallback, onResponseReturn);
        }
        
        public void RpcCall(string name, object data, Action callbackOnResponse = null)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            
            _rpc.RpcCall(name, data, callbackOnResponse);
        }
        
        public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            
            _rpc.RpcCall(name, data, mode, callbackOnResponse);
        }
        
        public void StartMatchmaking(Action callback = null)
        {
            _playroomService.StartMatchmaking(callback);
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
        
        public static Dictionary<string, Player> GetPlayers()
        {
            return Players;
        }
        
        public string GetRoomCode()
        {
            return _playroomService.GetRoomCode();
        }
        
        
        public  void OnDisconnect(Action callback)
        {
            _playroomService.OnDisconnect(callback);
        }
        
        // DI END
        

        public class MatchMakingOptions
        {
            public int waitBeforeCreatingNewRoom = 5000;
        }
        

        private static List<Action<Player>> OnPlayerJoinCallbacks = new();


        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void __OnPlayerJoinCallbackHandler(string id)
        {
            OnPlayerJoinWrapperCallback(id);
        }
        

        private static void OnPlayerJoinWrapperCallback(string id)
        {
            var player = GetPlayer(id);
            foreach (var callback in OnPlayerJoinCallbacks)
            {
                callback?.Invoke(player);
            }
        }
        
        public static bool IsStreamScreen()
        {
            if (IsRunningInBrowser())
            {
                return IsStreamScreenInternal();
            }

            if (isPlayRoomInitialized) return MockIsStreamScreen();
            Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
            return false;
        }
        

        public static Player MyPlayer()
        {
            if (IsRunningInBrowser())
            {
                var id = MyPlayerInternal();
                return GetPlayer(id);
            }

            if (isPlayRoomInitialized) return MockMyPlayer();
            Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
            return null;
        }

        public static Player Me()
        {
            return IsRunningInBrowser() ? MyPlayer() : MockMe();
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
            else
            {
                MockWaitForState(stateKey, onStateSetCallback);
            }
        }


        Action WaitForPlayerCallback = null;

        public void WaitForPlayerState(string playerID, string stateKey, Action onStateSetCallback = null)
        {
            if (IsRunningInBrowser())
            {
                WaitForPlayerCallback = onStateSetCallback;
                WaitForPlayerStateInternal(playerID, stateKey, OnStateSetCallback);
            }
        }

        [MonoPInvokeCallback(typeof(Action))]
        void OnStateSetCallback()
        {
            WaitForPlayerCallback?.Invoke();
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
            else
            {
                MockResetStates(keysToExclude);
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
            else
            {
                MockResetPlayersStates(keysToExclude, OnStatesReset);
            }
        }


       



        private static void UnsubscribeOnQuit()
        {
            UnsubscribeOnQuitInternal();
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
        
    }
}