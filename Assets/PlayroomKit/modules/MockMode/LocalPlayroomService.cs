using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class LocalMockPlayroomService : IPlayroomBase
        {
            private Dictionary<string, object> mockGlobalStates = new();

            private const string PlayerId = "mockplayerID123";

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
        }
    }
}