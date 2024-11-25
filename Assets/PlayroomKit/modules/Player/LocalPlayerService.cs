using System;
using System.Collections.Generic;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public partial class Player
        {
            public class LocalPlayerService : IPlayerBase
            {
                private string _id;

                public LocalPlayerService(string id)
                {
                    _id = id;
                }

                private Dictionary<(string playerID, string stateKey), Action> stateCallbacks = new();
                private static Dictionary<string, object> mockPlayerStatesDictionary = new();


                public static Dictionary<string, object> GetMockPlayerStates()
                {
                    return mockPlayerStatesDictionary;
                }

                public void SetStateHelper<T>(string key, T value, bool reliable = false)
                {
                    DebugLogger.Log($"MockPlayerService setState: {key} => {value}");
                    if (mockPlayerStatesDictionary.ContainsKey(key))
                        mockPlayerStatesDictionary[key] = value;
                    else
                        mockPlayerStatesDictionary.Add(key, value);

                    CallbackManager.InvokeCallback($"{key}_{_id}", value.ToString());
                }

                public void SetState(string key, int value, bool reliable = false)
                {
                    SetStateHelper(key, value, reliable);
                }

                public void SetState(string key, float value, bool reliable = false)
                {
                    SetStateHelper(key, value, reliable);
                }

                public void SetState(string key, bool value, bool reliable = false)
                {
                    SetStateHelper(key, value, reliable);
                }

                public void SetState(string key, string value, bool reliable = false)
                {
                    SetStateHelper(key, value, reliable);
                }

                public void SetState(string key, object value, bool reliable = false)
                {
                    SetStateHelper(key, value, reliable);
                }

                public T GetState<T>(string key)
                {
                    if (mockPlayerStatesDictionary.TryGetValue(key, out var value) && value is T typedValue)
                    {
                        return typedValue;
                    }
                    else
                    {
                        Debug.LogWarning($"No {key} in States or value is not of type {typeof(T)}");
                        return default;
                    }
                }

                public Profile GetProfile()
                {
                    Profile.PlayerProfileColor mockPlayerProfileColor = new()
                    {
                        r = 166,
                        g = 0,
                        b = 142,
                        hexString = "#a6008e"
                    };
                    ColorUtility.TryParseHtmlString(mockPlayerProfileColor.hexString, out UnityEngine.Color color1);
                    var testProfile = new Profile()
                    {
                        color = color1,
                        name = "MockPlayer",
                        playerProfileColor = mockPlayerProfileColor,
                        photo = "testPhoto"
                    };
                    return testProfile;
                }

                public Action OnQuit(Action<string> callback)
                {
                    DebugLogger.Log($"OnQuit is not supported in Local Mock Mode.");
                    return null;
                }

                public void Kick(Action onKickCallBack = null)
                {
                    var player = GetPlayerById(_id);
                    Players.Remove(player.id);
                    IPlayerBase.onKickCallBack?.Invoke();
                }

                public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
                {
                    if (onStateSetCallback == null)
                        return;

                    string key = $"{stateKey}_{_id}";
                    CallbackManager.RegisterCallback(onStateSetCallback, key);
                    DebugLogger.Log($"Callback registered, the key is: {key}");
                }
            }
        }
    }
}