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
            throw new NotImplementedException();
        }

        public void StartMatchmaking(Action callback = null)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnect(Action callback)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Local Player Mehtods

        public PlayroomKit.Player MyPlayer()
        {
            throw new NotImplementedException();
        }

        public PlayroomKit.Player Me()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Room Mehtods

        public bool IsHost()
        {
            throw new NotImplementedException();
        }

        public string GetRoomCode()
        {
            throw new NotImplementedException();
        }

        public bool IsStreamScreen()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region State Syncing Methods

        public void SetState<T>(string key, T value, bool reliable = false)
        {
            throw new NotImplementedException();
        }

        public T GetState<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            throw new NotImplementedException();
        }

        public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null)
        {
            throw new NotImplementedException();
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