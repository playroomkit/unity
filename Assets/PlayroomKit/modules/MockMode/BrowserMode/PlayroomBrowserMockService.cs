using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UBB;
using UnityEngine;

namespace Playroom
{
    public class PlayroomBrowserMockService : PlayroomKit.IPlayroomBase
    {
        private UnityBrowserBridge _ubb;

        private void CallJs(string jsFunctionName, string callbackName = null, string gameObjectName = null,
            bool isAsync = false, params string[] args)
        {
            List<string> allParams = new List<string>(args);
            if (!string.IsNullOrEmpty(callbackName)) allParams.Add($"'{callbackName}'");
            if (!string.IsNullOrEmpty(gameObjectName)) allParams.Add($"'{gameObjectName}'");

            string jsCall = $"{jsFunctionName}({string.Join(", ", allParams)})";
            if (isAsync) jsCall = $"await {jsCall}";

            _ubb.ExecuteJS(jsCall);
        }


        private T CallJs<T>(string jsFunctionName, string callbackName = null, string gameObjectName = null,
            bool isAsync = false, params string[] args)
        {
            List<string> allParams = new List<string>(args);
            if (!string.IsNullOrEmpty(callbackName)) allParams.Add($"'{callbackName}'");
            if (!string.IsNullOrEmpty(gameObjectName)) allParams.Add($"'{gameObjectName}'");

            string jsCall = $"{jsFunctionName}({string.Join(", ", allParams)})";
            if (isAsync) jsCall = $"await {jsCall}";

            return _ubb.ExecuteJS<T>(jsCall);
        }

        public static void MockOnPlayerJoinWrapper(string playerId)
        {
            PlayroomKit.IPlayroomBase.OnPlayerJoinWrapperCallback(playerId);
        }

        #region Initialization Methods

        public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
            Action onDisconnectCallback = null)
        {
            // start ubb before inserting coin
            _ubb = UnityBrowserBridge.Instance;
            _ubb.StartUBB();

            string optionsJson = null;
            if (options != null) optionsJson = Helpers.SerializeInitOptions(options);

            var gameObjectName = _ubb.GetGameObject("InsertCoin").name;
            CallJs("InsertCoin", onLaunchCallBack.GetMethodInfo().Name, gameObjectName, true, optionsJson);
            PlayroomKit.IsPlayRoomInitialized = true;
        }

        public Action OnPlayerJoin(Action<PlayroomKit.Player> onPlayerJoinCallback)
        {
            if (!PlayroomKit.IPlayroomBase.OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback))
                PlayroomKit.IPlayroomBase.OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);

            CallJs("OnPlayerJoin", null, _ubb.GetGameObject("devManager").name);

            void Unsub()
            {
                Debug.Log("Unsubscribing from OnPlayerJoin");
            }

            return Unsub;
        }

        public void StartMatchmaking(Action callback = null)
        {
            CallJs("StartMatchmaking", null, null, true);
            callback?.Invoke();
        }

        public void OnDisconnect(Action callback)
        {
            string key = Guid.NewGuid().ToString();
            string callbackKey = $"OnDisconnect_{key}";
            GameObject callbackObject = new GameObject(callbackKey);
            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(callback, callbackKey);
            CallJs("OnDisconnect", callbackKey);
        }

        #endregion

        #region Local Player Mehtods

        public PlayroomKit.Player MyPlayer()
        {
            string id = CallJs<string>("MyPlayer");
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
            return CallJs<bool>("IsHost");
        }

        public string GetRoomCode()
        {
            return CallJs<string>("GetRoomCode");
        }

        public bool IsStreamScreen()
        {
            return CallJs<bool>("IsStreamScreen");
        }

        #endregion

        #region State Syncing Methods

        public void SetState<T>(string key, T value, bool reliable = false)
        {
            CallJs("SetState", null, null, true, key, value.ToString(), reliable.ToString().ToLower());
        }

        public T GetState<T>(string key)
        {
            return CallJs<T>("GetState", null, null, true, key);
        }

        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            string callbackKey = $"WaitForState_{stateKey}";
            GameObject callbackObject = new GameObject(callbackKey);

            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(onStateSetCallback, callbackKey);

            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");

            CallJs("WaitForState", null, null, true, stateKey, callbackKey);
        }

        public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null)
        {
            string callbackKey = $"WaitForPlayerState_{stateKey}";
            GameObject callbackObject = new GameObject(callbackKey);

            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(onStateSetCallback, callbackKey);

            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");

            CallJs("WaitForPlayerState", null, null, true, playerID, stateKey, callbackKey);
        }

        public void ResetStates(string[] keysToExclude = null, Action onStatesReset = null)
        {
            throw new NotImplementedException();
        }

        public void ResetPlayersStates(string[] keysToExclude = null, Action onStatesReset = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Joystick Mehtods

        public void CreateJoyStick(JoystickOptions options)
        {
            throw new NotImplementedException();
        }

        public Dpad DpadJoystick()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Misc

        public void UnsubscribeOnQuit()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}