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

                private static Dictionary<string, object> mockPlayerStatesDictionary = new();

                public void SetStateHelper<T>(string id, string key, T value, bool reliable = false)
                {
                    Debug.Log($"MockPlayerService setState: {key} => {value}");
                    if (mockPlayerStatesDictionary.ContainsKey(key))
                        mockPlayerStatesDictionary[key] = value;
                    else
                        mockPlayerStatesDictionary.Add(key, value);
                }

                public void SetState(string id, string key, int value, bool reliable = false)
                {
                    SetStateHelper(id, key, value, reliable);
                        
                }

                public void SetState(string id, string key, float value, bool reliable = false)
                {
                    SetStateHelper(id, key, value, reliable);
                }

                public void SetState(string id, string key, bool value, bool reliable = false)
                {
                    SetStateHelper(id, key, value, reliable);
                }

                public void SetState(string id, string key, string value, bool reliable = false)
                {
                    SetStateHelper(id, key, value, reliable);
                }

                public void SetState(string id, string key, object value, bool reliable = false)
                {
                    SetStateHelper(id, key, value, reliable);
                }

                public T GetState<T>(string id, string key)
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

                public Profile GetProfile(string id)
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
                    throw new NotImplementedException();
                }

                public void Kick(string id, Action OnKickCallBack = null)
                {
                    var player = GetPlayer(PlayerId);
                    Players.Remove(player.id);
                    IPlayerBase.onKickCallBack?.Invoke();
                }

                public void WaitForState(string id, string StateKey, Action onStateSetCallback = null)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
    
}