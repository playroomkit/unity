using System;
using System.Collections.Generic;
using System.Reflection;
using UBB;
using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
using SimpleJSON;
#endif

namespace Playroom
{
#if UNITY_EDITOR
    public class BrowserMockService : PlayroomKit.IPlayroomBase
    {
        private UnityBrowserBridge _ubb;

        #region Initialization Methods

        public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
            Action onDisconnectCallback = null)
        {
            // start ubb before inserting coin
            _ubb = UnityBrowserBridge.Instance;
            if (ClonesManager.IsClone())
            {
                _ubb.httpServerPort += 10;
            }

            _ubb.StartUBB();

            string optionsJson = null;

            if (options.turnBased is bool turnBased)
            {
                if (turnBased)
                {
                    options.persistentMode = true;
                    optionsJson = Helpers.SerializeInitOptions(options);
                }
            }
            else if (options.turnBased is TurnBasedOptions turnBasedOptions)
            {
                if (!string.IsNullOrEmpty(turnBasedOptions.challengeId))
                {
                    options.persistentMode = true;
                    optionsJson = Helpers.SerializeInitOptions(options);
                }
            }
            else if (string.IsNullOrEmpty(options.roomCode) && !options.persistentMode)
            {
                options.roomCode = "TEST_ROOM";
                optionsJson = Helpers.SerializeInitOptions(options);
            }
            else
            {
                optionsJson = Helpers.SerializeInitOptions(options);
            }

            var gameObjectName = _ubb.GetGameObject("InsertCoin").name;
            var devManagerName = _ubb.GetGameObject("devManager").name;
            Debug.Log("DevManagerName:" + gameObjectName);

            Debug.Log(optionsJson);

            _ubb.CallJs("InsertCoin", onLaunchCallBack.GetMethodInfo().Name, gameObjectName, true, optionsJson);
            PlayroomKit.IsPlayRoomInitialized = true;
        }

        public Action OnPlayerJoin(Action<PlayroomKit.Player> onPlayerJoinCallback)
        {
            if (!PlayroomKit.IPlayroomBase.OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback))
                PlayroomKit.IPlayroomBase.OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);

            _ubb.CallJs("OnPlayerJoin", null, _ubb.GetGameObject("devManager").name);

            void Unsub()
            {
                DebugLogger.Log("Unsubscribing from OnPlayerJoin");
            }

            return Unsub;
        }

        public void StartMatchmaking(Action callback = null)
        {
            _ubb.CallJs("StartMatchmaking", null, null, true);
            callback?.Invoke();
        }

        public void OnDisconnect(Action callback)
        {
            string key = Guid.NewGuid().ToString();
            string callbackKey = $"OnDisconnect_{key}";
            GameObject callbackObject = new GameObject(callbackKey);
            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(callback, callbackKey);
            _ubb.CallJs("OnDisconnect", callbackKey);
        }

        #endregion

        #region Local Player Mehtods

        public PlayroomKit.Player MyPlayer()
        {
            string id = _ubb.CallJs<string>("MyPlayer");
            return PlayroomKit.GetPlayerById(id);
        }

        public PlayroomKit.Player Me()
        {
            return MyPlayer();
        }

        #endregion

        #region Room Mehtods

        public bool IsHost()
        {
            return _ubb.CallJs<bool>("IsHost");
        }

        public void TransferHost(string playerId)
        {
            _ubb.CallJs("TransferHost", null, null, true, playerId);
        }

        public string GetRoomCode()
        {
            return _ubb.CallJs<string>("GetRoomCode");
        }

        public bool IsStreamScreen()
        {
            return _ubb.CallJs<bool>("IsStreamScreen");
        }

        #endregion

        #region State Syncing Methods

        public void SetState<T>(string key, T value, bool reliable = false)
        {
            _ubb.CallJs("SetState", null, null, true,
                key, value.ToString(), reliable.ToString().ToLower());
        }

        public T GetState<T>(string key)
        {
            return _ubb.CallJs<T>("GetState", null, null, false, key);
        }


        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            string callbackKey = $"WaitForState_{stateKey}";
            GameObject callbackObject = new GameObject(callbackKey);

            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(onStateSetCallback, callbackKey);

            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");

            _ubb.CallJs("WaitForState", null, null, true, stateKey, callbackKey);
        }

        public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null)
        {
            string callbackKey = $"WaitForPlayerState_{stateKey}";
            GameObject callbackObject = new GameObject(callbackKey);
            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();

            invoker.SetCallback(onStateSetCallback, callbackKey);
            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");

            _ubb.CallJs("WaitForPlayerState", callbackKey, null, false, playerID, stateKey);
        }

        public void ResetStates(string[] keysToExclude = null, Action onStatesReset = null)
        {
            _ubb.CallJs("ResetStates", null, null, true, keysToExclude ?? Array.Empty<string>());
            onStatesReset?.Invoke();
        }

        public void ResetPlayersStates(string[] keysToExclude = null, Action onStatesReset = null)
        {
            _ubb.CallJs("ResetPlayersStates", null, null, true, keysToExclude ?? Array.Empty<string>());
            onStatesReset?.Invoke();
        }

        #endregion

        #region Persistent API

        public void SetPersistentData(string key, object value)
        {
            string jsonString = JsonUtility.ToJson(value);
            _ubb.CallJs("SetPersistentData", null, null, true, key, jsonString);
        }

        public void InsertPersistentData(string key, object value)
        {
            string jsonString = JsonUtility.ToJson(value);
            _ubb.CallJs("InsertPersistentData", null, null, true, key, jsonString);
        }

        public void GetPersistentData(string key, Action<string> onGetPersistentDataCallback)
        {
            string dataJson = _ubb.CallJs<string>("GetPersistentData", null, null, true, key);
            onGetPersistentDataCallback?.Invoke(dataJson);
        }

        #endregion

        #region Turn Based

        public string GetChallengeId()
        {
            return _ubb.CallJs<string>("GetChallengeId");
        }

        public void SaveMyTurnData(object data)
        {
            string jsonData;

            if (data is int || data is string || data is float)
            {
                jsonData = JSONNode.Parse(data.ToString()).ToString();
            }
            else
            {
                jsonData = JsonUtility.ToJson(data);
            }

            _ubb.CallJs("SaveMyTurnData", null, null, true, jsonData);
        }

        public void GetMyTurnData(Action<string> callback)
        {
            string data = _ubb.CallJs<string>("GetMyTurnData", null, null, true);
            callback.Invoke(data);
        }

        public void GetAllTurns(Action<string> callback)
        {
            string data = _ubb.CallJs<string>("GetAllTurns", null, null, true);
            callback.Invoke(data);
        }

        public void ClearTurns(Action callback)
        {
            _ubb.CallJs("ClearTurns", null, null, true);
            callback.Invoke();
        }

        #endregion


        #region Misc

        public void UnsubscribeOnQuit()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Joystick Mehtods

        public void CreateJoyStick(JoystickOptions options)
        {
            Debug.LogWarning("Create Joystick is not supported in mock mode!");
        }

        public Dpad DpadJoystick()
        {
            Debug.LogWarning("Dpad Joystick is not supported in mock mode!");
            return null;
        }

        #endregion

        #region Utils

        public static void MockOnPlayerJoinWrapper(string playerId)
        {
            PlayroomKit.IPlayroomBase.OnPlayerJoinWrapperCallback(playerId);
        }

        #endregion
    }
#endif
}