using UnityEngine;
using System.Globalization;
using AOT;
using SimpleJSON;
using System;
using System.Collections.Generic;


namespace Playroom
{
    public partial class PlayroomKit
    {
        public class PlayroomService : IPlayroomBase, IPlayroomBuildExtensions
        {
            private readonly IInterop _interop;

            public PlayroomService()
            {
                _interop = new PlayroomKitInterop();
            }

            public PlayroomService(IInterop interop)
            {
                _interop = interop;
            }


            public Action OnPlayerJoin(Action<Player> onPlayerJoinCallback)
            {
                if (!IPlayroomBase.OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback))
                {
                    IPlayroomBase.OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
                }

                var CallbackID = _interop.OnPlayerJoinWrapper(IPlayroomBase.__OnPlayerJoinCallbackHandler);

                void Unsubscribe()
                {
                    IPlayroomBase.OnPlayerJoinCallbacks.Remove(onPlayerJoinCallback);
                    UnsubscribeOnPlayerJoin(CallbackID);
                }

                return Unsubscribe;
            }


            public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
                Action onDisconnectCallback = null)
            {
                isPlayRoomInitialized = true;

                var onLaunchCallBackKey = CallbackManager.RegisterCallback(onLaunchCallBack, "onLaunchCallBack");
                var onDisconnectCallBackKey =
                    CallbackManager.RegisterCallback(onDisconnectCallback, "onDisconnectCallBack");

                string optionsJson = null;
                if (options != null) optionsJson = SerializeInitOptions(options);

                if (options.skipLobby == false)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                            WebGLInput.captureAllKeyboardInput = false;
#endif
                }

                _interop.InsertCoinWrapper(
                    optionsJson, InvokeInsertCoin, __OnQuitInternalHandler, onDisconnectCallbackHandler,
                    InvokeOnErrorInsertCoin, onLaunchCallBackKey, onDisconnectCallBackKey);
            }

            public bool IsHost()
            {
                return _interop.IsHostWrapper();
            }


            public string GetRoomCode()
            {
                return _interop.GetRoomCodeWrapper();
            }

            static Action startMatchmakingCallback = null;

            public void StartMatchmaking(Action callback = null)
            {
                startMatchmakingCallback = callback;
                _interop.StartMatchmakingWrapper(InvokeStartMatchmakingCallback);
            }

            public void SetState<T>(string key, T value, bool reliable = false)
            {
                //#Debug.Log("SetState "+ key + ", value is " + value + "of type " + value.GetType());
                // Use type checking to call the correct overload of SetStateWrapper
                if (value is string)
                {
                    _interop.SetStateStringWrapper(key, (string)(object)value, reliable);
                }
                else if (value is int)
                {
                    _interop.SetStateIntWrapper(key, (int)(object)value, reliable);
                }
                else if (value is bool)
                {
                    _interop.SetStateBoolWrapper(key, (bool)(object)value, reliable);
                }
                else if (value is float || value is double)
                {
                    float floatValue = (float)(object)value;
                    var floatAsString = floatValue.ToString(CultureInfo.InvariantCulture);
                    _interop.SetStateFloatWrapper(key, floatAsString, reliable); // Assumes float is serialized as string
                }
                else if (value is object) // For JSON-like object, you'd serialize it to JSON (example using Newtonsoft.Json)
                {
                    Debug.Log("SetState "+ key + ", value is " + value + "of type " + value.GetType());
                    string jsonString = JsonUtility.ToJson(value);
                    _interop.SetStateStringWrapper(key, jsonString, reliable);
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported type: {typeof(T).Name}");
                }
            }
            
            public void SetState<T>(string key, Dictionary<string, T> values, bool reliable = false)
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
                _interop.SetStateDictionaryWrapper(key, jsonString, reliable);
            }
            
            public void SetState(string key, string value, bool reliable = false)
            {
                _interop.SetStateStringWrapper(key, value, reliable);
            }

            public void SetState(string key, int value, bool reliable = false)
            {
                _interop.SetStateIntWrapper(key, value, reliable);
            }

            public void SetState(string key, bool value, bool reliable = false)
            {
                _interop.SetStateBoolWrapper(key, value, reliable);
            }

            public void SetState(string key, float value, bool reliable = false)
            {
                string floatAsString = value.ToString(CultureInfo.InvariantCulture);
                _interop.SetStateFloatWrapper(key, floatAsString, reliable);
            }

            public void SetState(string key, object value, bool reliable = false)
            {
                Debug.Log("SetState " + key + ", value is " + value + " of type " + value.GetType());
                string jsonString = JsonUtility.ToJson(value);
                _interop.SetStateStringWrapper(key, jsonString, reliable);
            }


            public T GetState<T>(string key)
            {
                Type type = typeof(T);
                if (type == typeof(int)) return (T)(object)GetStateInt(key);
                else if (type == typeof(float)) return (T)(object)GetStateFloat(key);
                else if (type == typeof(bool)) return (T)(object)GetStateBool(key);
                else if (type == typeof(string)) return (T)(object)GetStateString(key);
                else if (type == typeof(Vector2)) return JsonUtility.FromJson<T>(GetStateString(key));
                else if (type == typeof(Vector3)) return JsonUtility.FromJson<T>(GetStateString(key));
                else if (type == typeof(Vector4)) return JsonUtility.FromJson<T>(GetStateString(key));
                else if (type == typeof(Quaternion)) return JsonUtility.FromJson<T>(GetStateString(key));
                else
                {
                    Debug.LogError($"GetState<{type}> is not supported.");
                    return default;
                }
            }


            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeStartMatchmakingCallback()
            {
                startMatchmakingCallback?.Invoke();
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void InvokeInsertCoin(string key)
            {
                CallbackManager.InvokeCallback(key);


#if UNITY_WEBGL && !UNITY_EDITOR
                WebGLInput.captureAllKeyboardInput = true;
#endif
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void __OnQuitInternalHandler(string playerId)
            {
                if (Players.TryGetValue(playerId, out Player player))
                {
                    throw new NotImplementedException(); //implement OnQuitWrapperCallback;
                }
                else
                {
                    Debug.LogError("[__OnQuitInternalHandler] Couldn't find player with id " + playerId);
                }
            }
            
            public void OnDisconnect(Action callback)
            {
                CallbackManager.RegisterCallback(callback);
                _interop.OnDisconnectWrapper(onDisconnectCallbackHandler);
            }

            public bool IsStreamScreen()
            {
                return _interop.IsStreamScreenWrapper();
            }

            public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
            {
                CallbackManager.RegisterCallback(onStateSetCallback, stateKey);
                _interop.WaitForStateWrapper(stateKey, IPlayroomBase.InvokeCallback);
            }
            
            
            Action WaitForPlayerCallback = null;
            public void WaitForPlayerState(string playerID, string stateKey, Action onStateSetCallback = null)
            {
                WaitForPlayerCallback = onStateSetCallback;
                _interop.WaitForPlayerStateWrapper(playerID, stateKey, OnStateSetCallback);
            }
            
            [MonoPInvokeCallback(typeof(Action))]
            void OnStateSetCallback()
            {
                WaitForPlayerCallback?.Invoke();
            }
            
            private static Action onstatesReset;
            private static Action onplayersStatesReset;

            public void ResetStates(string[] keysToExclude = null, Action OnStatesReset = null)
            {
                onstatesReset = OnStatesReset;
                string keysJson = keysToExclude != null ? CreateJsonArray(keysToExclude).ToString() : null;
                _interop.ResetStatesWrapper(keysJson, InvokeResetCallBack);
            }
            
            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeResetCallBack()
            {
                onstatesReset?.Invoke();
            }
            

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void onDisconnectCallbackHandler(string key)
            {
                CallbackManager.InvokeCallback(key);
            }

            private static Action<string> onError;

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void InvokeOnErrorInsertCoin(string error)
            {
                onError?.Invoke(error);
                Debug.LogException(new Exception(error));
            }


            private void UnsubscribeOnPlayerJoin(string callbackID)
            {
                _interop.UnsubscribeOnPlayerJoinWrapper(callbackID);
            }
            
            // GETTERS
            private string GetStateString(string key)
            {
                return _interop.GetStateStringWrapper(key);
            }


            private int GetStateInt(string key)
            {
                return _interop.GetStateIntWrapper(key);
            }


            private float GetStateFloat(string key)
            {
                return _interop.GetStateFloatWrapper(key);
            }

            private bool GetStateBool(string key)
            {
                var stateValue = GetStateInt(key);
                return stateValue == 1 ? true :
                    stateValue == 0 ? false :
                    throw new InvalidOperationException($"GetStateBool: {key} is not a bool");
            }
        }
    }
}