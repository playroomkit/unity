using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class LocalMockPlayroomService : IPlayroomBase
        {
            private Dictionary<string, object> mockGlobalStates = new();

            // private Dictionary<string, Action<string>> stateCallbacks = new();

            private const string PlayerId = "mockplayerID123";

            private static bool mockIsStreamMode;
            private Dictionary<string, string> mockGlobalStatesDictionary = new();
            private Dictionary<string, string> mockPlayerStatesDictionary = new();

            public Action OnPlayerJoin(Action<Player> onPlayerJoinCallback)
            {
                Debug.Log("On Player Join");
                var testPlayer = GetPlayer(PlayerId);
                IPlayroomBase.OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
                IPlayroomBase.__OnPlayerJoinCallbackHandler(PlayerId);

                void Unsubscribe()
                {
                    IPlayroomBase.OnPlayerJoinCallbacks.Remove(onPlayerJoinCallback);
                }

                return Unsubscribe;
            }

            public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
                Action onDisconnectCallback = null)
            {
                isPlayRoomInitialized = true;
                Debug.Log("Coin Inserted");
                string optionsJson = null;
                if (options != null) optionsJson = SerializeInitOptions(options);
                onLaunchCallBack?.Invoke();
            }

            public Player MyPlayer()
            {
                return GetPlayer(PlayerId);
            }

            public Player Me()
            {
                return GetPlayer(PlayerId);
            }

            public bool IsHost()
            {
                return true;
            }

            public string GetRoomCode()
            {
                return "mock123";
            }

            public void StartMatchmaking(Action callback = null)
            {
                Debug.Log("Matchmaking doesn't work in local mock mode!");
                callback?.Invoke();
            }

            public void SetState<T>(string key, T value, bool reliable = false)
            {
                if (mockGlobalStates.ContainsKey(key))
                    mockGlobalStates[key] = value;
                else
                    mockGlobalStates.Add(key, value);

                CallbackManager.InvokeCallback(key, value as string);
            }

            public T GetState<T>(string key)
            {
                if (mockGlobalStates.TryGetValue(key, out var value) && value is T typedValue)
                {
                    try
                    {
                        // Attempt to convert the string to the expected type T
                        return (T)Convert.ChangeType(typedValue, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        Debug.LogWarning($"Failed to convert the value of key '{key}' to type {typeof(T)}.");
                        return default;
                    }
                }
                else
                {
                    Debug.LogWarning($"No {key} in States");
                    return default;
                }
            }

            public void OnDisconnect(Action callback)
            {
                callback?.Invoke();
            }

            public bool IsStreamScreen()
            {
                return mockIsStreamMode;
            }

            public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
            {
                if (onStateSetCallback == null)
                    return; 

                CallbackManager.RegisterCallback(onStateSetCallback, stateKey);
            }

            public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null)
            {
                CallbackManager.RegisterCallback(onStateSetCallback, $"{stateKey}_{playerID}");
            }

            public void ResetStates(string[] keysToExclude = null, Action OnStatesReset = null)
            {
                List<string> keysToRemove =
                    mockGlobalStatesDictionary.Keys.Where(key => !keysToExclude.Contains(key)).ToList();
                foreach (string key in keysToRemove) mockGlobalStatesDictionary.Remove(key);
                OnStatesReset?.Invoke();
            }

            public void ResetPlayersStates(string[] keysToExclude = null, Action OnStatesReset = null)
            {
                if (keysToExclude == null || keysToExclude.Length == 0)
                {
                    keysToExclude = Array.Empty<string>();
                }

                List<string> keysToRemove =
                    mockPlayerStatesDictionary.Keys.Where(key => !keysToExclude.Contains(key)).ToList();

                foreach (string key in keysToRemove)
                {
                    mockPlayerStatesDictionary.Remove(key);
                }

                OnStatesReset?.Invoke();
            }

            public void CreateJoyStick(JoystickOptions options)
            {
                throw new NotImplementedException();
            }

            public Dpad DpadJoystick()
            {
                throw new NotImplementedException();
            }

            public void UnsubscribeOnQuit()
            {
                throw new NotImplementedException();
            }
        }
    }
}