using System;
using System.Collections.Generic;
using AOT;
using Playroom;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public interface IPlayroomBase
        {
            protected static List<Action<Player>> OnPlayerJoinCallbacks = new();

            public Action OnPlayerJoin(Action<Player> onPlayerJoinCallback);

            public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
                Action onDisconnectCallback = null);

            public Player MyPlayer();

            public Player Me();

            public bool IsHost();

            public string GetRoomCode();

            public void StartMatchmaking(Action callback = null);

            public void SetState<T>(string key, T value, bool reliable = false);

            public T GetState<T>(string key);

            public void OnDisconnect(Action callback);

            public bool IsStreamScreen();

            public void WaitForState(string stateKey, Action<string> onStateSetCallback = null);

            public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null);

            public void ResetStates(string[] keysToExclude = null, Action onStatesReset = null);

            public void ResetPlayersStates(string[] keysToExclude = null, Action onStatesReset = null);

            public void CreateJoyStick(JoystickOptions options);

            public Dpad DpadJoystick();

            public void UnsubscribeOnQuit();

            [MonoPInvokeCallback(typeof(Action<string>))]
            protected static void __OnPlayerJoinCallbackHandler(string id)
            {
                OnPlayerJoinWrapperCallback(id);
            }

            protected static void OnPlayerJoinWrapperCallback(string id)
            {
                var player = GetPlayerById(id);
                foreach (var callback in IPlayroomBase.OnPlayerJoinCallbacks)
                {
                    callback?.Invoke(player);
                }
            }
            
            
            [MonoPInvokeCallback(typeof(Action<string, string>))]
            protected static void InvokeCallback(string stateKey, string stateVal)
            {
                CallbackManager.InvokeCallback(stateKey, stateVal);
            }
        }
    }
}