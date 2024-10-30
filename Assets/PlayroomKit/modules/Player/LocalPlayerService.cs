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

                public void SetStateHelper<T>(string key, T value, bool reliable = false)
                {
                    Debug.Log($"MockPlayerService setState: {key} => {value}");
                    if (mockPlayerStatesDictionary.ContainsKey(key))
                        mockPlayerStatesDictionary[key] = value;
                    else
                        mockPlayerStatesDictionary.Add(key, value);
                    
                    var callbackkey = (_id, key);
                    if (stateCallbacks.TryGetValue(callbackkey, out var callback))
                    {
                        // Invoke the callback and remove it from the dictionary (assuming one-time trigger)
                        callback.Invoke();
                        stateCallbacks.Remove(callbackkey);
                        Debug.Log($"Callback invoked for PlayerID: {_id}, StateKey: {key}");
                    }
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
                    Debug.Log($"OnQuit is not implemented for local");
                    return null;
                }

                public void Kick(Action OnKickCallBack = null)
                {
                    var player = GetPlayer(PlayerId);
                    Players.Remove(player.id);
                    IPlayerBase.onKickCallBack?.Invoke();
                }

                public void WaitForState(string stateKey, Action onStateSetCallback = null)
                {
                    if (onStateSetCallback == null)
                        return; // No callback provided, nothing to register

                    // Register the callback in the stateCallbacks dictionary
                    var key = (_id, stateKey);
                    stateCallbacks[key] = onStateSetCallback;

                    Debug.Log($"Callback registered for PlayerID: {_id}, StateKey: {stateKey}");
                }
            }
        }
    }
    
}