using UnityEngine;
using System.Globalization;
using AOT;
using SimpleJSON;
using System;
using System.Collections.Generic;
using Discord;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class PlayroomBuildService : IPlayroomBase, IPlayroomBuildExtensions
        {
            private readonly IInterop _interop;
            private static Action<string> _onError;
            private static Action _onStatesResetCallback;
            private static Action _onPlayersStatesResetCallback;

            public PlayroomBuildService()
            {
                _interop = new PlayroomKitInterop();
            }

            public PlayroomBuildService(IInterop interop)
            {
                _interop = interop;
            }

            #region Init Methods

            public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
                Action onDisconnectCallback = null)
            {
                IsPlayRoomInitialized = true;

                var onLaunchCallBackKey = CallbackManager.RegisterCallback(onLaunchCallBack, "onLaunchCallBack");
                var onDisconnectCallBackKey =
                    CallbackManager.RegisterCallback(onDisconnectCallback, "onDisconnectCallBack");

                string optionsJson = null;
                if (options != null) optionsJson = Helpers.SerializeInitOptions(options);

                if (options.skipLobby == false)
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                            WebGLInput.captureAllKeyboardInput = false;
#endif
                }

                if (options.turnBased is true)
                {
                    options.persistentMode = true;
                }
                else if (options.turnBased is TurnBasedOptions turnBasedOptions)
                {
                    if (!string.IsNullOrEmpty(turnBasedOptions.challengeId))
                    {
                        options.persistentMode = true;
                    }
                }

                _interop.InsertCoinWrapper(
                    optionsJson, InvokeInsertCoin, IPlayroomBase.__OnQuitInternalHandler, OnDisconnectCallbackHandler,
                    InvokeOnErrorInsertCoin, onLaunchCallBackKey, onDisconnectCallBackKey);
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


            public void StartMatchmaking(Action callback = null)
            {
                CallbackManager.RegisterCallback(callback, "matchMakingStarted");
                _interop.StartMatchmakingWrapper(InvokeStartMatchmakingCallback);
            }

            public void OnDisconnect(Action callback)
            {
                CallbackManager.RegisterCallback(callback);
                _interop.OnDisconnectWrapper(OnDisconnectCallbackHandler);
            }

            public string GetPlayroomToken()
            {
                return GetPlayroomTokenInternal();
            }

            #endregion

            #region Unsubscribers

            public void UnsubscribeOnQuit()
            {
                _interop.UnsubscribeOnQuitWrapper();
            }

            private void UnsubscribeOnPlayerJoin(string callbackID)
            {
                _interop.UnsubscribeOnPlayerJoinWrapper(callbackID);
            }

            #endregion

            #region Local Player

            public Player MyPlayer()
            {
                var id = _interop.MyPlayerWrapper();
                return GetPlayerById(id);
            }

            public Player Me()
            {
                return MyPlayer();
            }

            #endregion

            #region Room

            public bool IsHost()
            {
                return _interop.IsHostWrapper();
            }

            public void TransferHost(string playerId)
            {
                _interop.TransferHostWrapper(playerId);
            }

            public string GetRoomCode()
            {
                return _interop.GetRoomCodeWrapper();
            }

            public bool IsStreamScreen()
            {
                return _interop.IsStreamScreenWrapper();
            }

            #endregion

            #region State

            public void SetState<T>(string key, T value, bool reliable = false)
            {
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
                    _interop.SetStateFloatWrapper(key, floatAsString, reliable);
                }
                else if (value is Enum)
                {
                    _interop.SetStateStringWrapper(key, value.ToString(), reliable);
                }
                else if (value is object)
                {
                    string jsonString = JsonUtility.ToJson(value);
                    Debug.Log("SetState " + key + ", value is " + value + "of type " + value.GetType());
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

                foreach (var kvp in values)
                {
                    var value = Convert.ToDouble(kvp.Value);
                    jsonObject.Add(kvp.Key, value);
                }

                var jsonString = jsonObject.ToString();
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
                string jsonString = JsonUtility.ToJson(value);
                _interop.SetStateStringWrapper(key, jsonString, reliable);
            }

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
                else if (type.IsEnum)
                {
                    try
                    {
                        string valueString = _interop.GetStateStringWrapper(key);

                        if (string.IsNullOrEmpty(valueString))
                        {
                            return default;
                        }

                        return (T)Enum.Parse(typeof(T), valueString.Trim('\"', ' '));
                    }
                    catch (ArgumentException)
                    {
                        Debug.LogError($"Failed to parse '{key}' to Enum of type {typeof(T)}");
                        return default;
                    }
                }
                else
                {
                    Debug.LogError($"GetState<{type}> is not supported.");
                    return default;
                }
            }


            public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
            {
                CallbackManager.RegisterCallback(onStateSetCallback, stateKey);
                _interop.WaitForStateWrapper(stateKey, IPlayroomBase.InvokeCallback);
            }

            Action<string> WaitForPlayerCallback = null;

            public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null)
            {
                WaitForPlayerCallback = onStateSetCallback;
                _interop.WaitForPlayerStateWrapper(playerID, stateKey, OnStateSetCallback);
            }

            public void ResetStates(string[] keysToExclude = null, Action onStatesReset = null)
            {
                _onStatesResetCallback = onStatesReset;
                string keysJson = keysToExclude != null ? Helpers.CreateJsonArray(keysToExclude).ToString() : null;
                _interop.ResetStatesWrapper(keysJson, InvokeResetCallBack);
            }

            public void ResetPlayersStates(string[] keysToExclude = null, Action onStatesReset = null)
            {
                _onStatesResetCallback = onStatesReset;
                string keysJson = keysToExclude != null ? Helpers.CreateJsonArray(keysToExclude).ToString() : null;
                _interop.ResetPlayersStatesWrapper(keysJson, InvokePlayersResetCallBack);
            }

            #endregion

            #region Joystick

            public void CreateJoyStick(JoystickOptions options)
            {
                string jsonStr = Helpers.ConvertJoystickOptionsToJson(options);
                _interop.CreateJoystickWrapper(jsonStr);
            }

            public Dpad DpadJoystick()
            {
                var jsonString = DpadJoystickInternal();
                Dpad myDpad = JsonUtility.FromJson<Dpad>(jsonString);
                return myDpad;
            }

            #endregion

            #region Persistent API

            public void SetPersistentData(string key, object value)
            {
                string jsonString = Helpers.SerializeObject(value);
                _interop.SetPersistentDataWrapper(key, jsonString);
            }

            public void InsertPersistentData(string key, object value)
            {
                string jsonString = Helpers.SerializeObject(value);
                _interop.InsertPersistentDataWrapper(key, jsonString);
            }

            public void GetPersistentData(string key, Action<string> onGetPersistentDataCallback)
            {
                CallbackManager.RegisterCallback(onGetPersistentDataCallback, key);
                _interop.GetPersistentDataWrapper(key, IPlayroomBase.InvokeCallback);
            }

            #endregion

            #region TurnBased API

            public string GetChallengeId()
            {
                CheckPlayRoomInitialized();
                return _interop.GetChallengeIdWrapper();
            }

            public void SaveMyTurnData(object data)
            {
                CheckPlayRoomInitialized();

                string jsonData;

                if (data is int || data is string || data is float)
                    jsonData = JSONNode.Parse(data.ToString()).ToString();
                else
                    jsonData = JsonUtility.ToJson(data);

                _interop.SaveMyTurnDataWrapper(jsonData);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void TurnBasedMyDataCallback(string data)
            {
                // TODO Convert json data to object:
                TurnData turnData = Helpers.ParseTurnData(data);

                CallbackManager.InvokeCallback("GetMyData", turnData);
            }

            public void GetMyTurnData(Action<TurnData> callback)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "GetMyData");
                _interop.GetMyTurnDataWrapper(TurnBasedMyDataCallback);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void turnBasedCallbackHandler(string data)
            {
                List<TurnData> turnData = Helpers.ParseAllTurnData(data);
                CallbackManager.InvokeCallback("GetAllTurns", turnData);
            }

            public void GetAllTurns(Action<List<TurnData>> callback)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "GetAllTurns");
                _interop.GetAllTurnsWrapper(turnBasedCallbackHandler);
            }

            public void ClearTurns(Action callback = null)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "clearTurns");
                _interop.ClearTurnsWrapper(ClearTurnsCallback);
            }

            [MonoPInvokeCallback(typeof(Action))]
            private static void ClearTurnsCallback()
            {
                CallbackManager.InvokeCallback("clearTurns");
            }

            #endregion

            #region Discord API
            public void OpenDiscordInviteDialog(Action callback = null)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "discordInviteDialog");
                _interop.OpenDiscordInviteDialogInternalWrapper(OpenDiscordInviteDialogCallbackInvoker);
            }

            public void OpenDiscordExternalLink(string url, Action<string> callback = null)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "discordExternalLink");
                OpenDiscordExternalLinkInternal(url, InvokeOpenDiscordExternalLinkCallback);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void InvokeOpenDiscordExternalLinkCallback(string openedString)
            {
                CallbackManager.InvokeCallback("discordExternalLink", openedString);
            }

            [MonoPInvokeCallback(typeof(Action<string, string>))]
            private static void StartDiscordPurchaseCallback(string skuId, string result)
            {
                CallbackManager.InvokeCallback(skuId, result);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void OnErrorStartPurchase(string errorLog)
            {
                CallbackManager.InvokeCallback("onError", errorLog);
            }

            public void StartDiscordPurchase(string skuId, Action<string> callback = null, Action<string> onError = null)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, skuId);
                CallbackManager.RegisterCallback(onError, "onError");

                StartDiscordPurchaseInternal(skuId, StartDiscordPurchaseCallback, OnErrorStartPurchase);
            }

            public void GetDiscordSkus(Action<List<DiscordSku>> callback)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "discordSkus");
                GetDiscordSkusInternal(DiscordSkusCallback);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void DiscordSkusCallback(string data)
            {
                try
                {
                    List<DiscordSku> list = DiscordSku.FromJSON(data);
                    CallbackManager.InvokeCallback("discordSkus", list);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Unity]: Error in Discord SKU: {e}");
                    throw;
                }
            }

            public void GetDiscordEntitlements(Action<List<DiscordEntitlement>> callback)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "discordEntitlements");
                GetDiscordEntitlementsInternal(DiscordEntitlementsCallback);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void DiscordEntitlementsCallback(string data)
            {
                try
                {
                    List<DiscordEntitlement> list = DiscordEntitlement.FromJSON(data);
                    CallbackManager.InvokeCallback("discordEntitlements", list);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Unity]: Error in discord Entitlements: {e}");
                    throw;
                }
            }

            public void DiscordPriceFormat(float price, string currency, string locale, Action<string> callback)
            {
                CheckPlayRoomInitialized();
                CallbackManager.RegisterCallback(callback, "formattedPrice");
                DiscordPriceFormatInternal(price, currency, locale, DiscordPriceFormatCallbackInvoker);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void DiscordPriceFormatCallbackInvoker(string formattedPrice)
            {
                CallbackManager.InvokeCallback("formattedPrice", formattedPrice);
            }

            public void SubscribeDiscordEvent(SDKEvent eventName, Action<string> callback)
            {
                CallbackManager.RegisterCallback(callback, eventName.ToString());
                SubscribeDiscordInternal(eventName.ToString(), InvokeSubscribeDiscord);
            }

            [MonoPInvokeCallback(typeof(Action<string, string>))]
            private static void InvokeSubscribeDiscord(string eventName, string data)
            {
                CallbackManager.InvokeCallback(eventName, data);
            }
            #endregion

            #region Callbacks

            [MonoPInvokeCallback(typeof(Action))]
            private static void OpenDiscordInviteDialogCallbackInvoker()
            {
                CallbackManager.InvokeCallback("discordInviteDialog");
            }

            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeStartMatchmakingCallback()
            {
                CallbackManager.InvokeCallback("matchMakingStarted");
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
            void OnStateSetCallback(string data)
            {
                WaitForPlayerCallback?.Invoke(data);
            }

            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeResetCallBack()
            {
                _onStatesResetCallback?.Invoke();
            }

            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokePlayersResetCallBack()
            {
                _onPlayersStatesResetCallback?.Invoke();
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void OnDisconnectCallbackHandler(string key)
            {
                CallbackManager.InvokeCallback(key);
            }

            [MonoPInvokeCallback(typeof(Action<string>))]
            private static void InvokeOnErrorInsertCoin(string error)
            {
                _onError?.Invoke(error);
                Debug.LogException(new Exception(error));
            }

            #endregion
        }
    }
}