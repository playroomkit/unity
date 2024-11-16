using System;
using System.Collections.Generic;
using System.Reflection;
using UBB;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;


namespace Playroom
{
    /// <summary>
    ///     This file contains the new  mock-mode which uses browser-driver to run Playroom's within the editor!.
    /// </summary>
    public partial class PlayroomKit
    {
        // Store GameObjects by function type
        private static readonly Dictionary<string, GameObject> gameObjectReferences = new();

        public static void RegisterGameObject(string key, GameObject gameObject)
        {
            if (!gameObjectReferences.ContainsKey(key))
                gameObjectReferences.Add(key, gameObject);
            else
                gameObjectReferences[key] = gameObject;
        }

        public static GameObject GetGameObject(string key)
        {
            gameObjectReferences.TryGetValue(key, out var gameObject);
            return gameObject;
        }


        private static void MockInsertCoinBrowser(InitOptions options, Action onLaunchCallBack)
        {
            IsPlayRoomInitialized = true;

            Debug.Log("Coin Inserted!");

            string optionsJson = null;
            if (options != null) optionsJson = Helpers.SerializeInitOptions(options);

#if UNITY_EDITOR
            var gameObjectName = GetGameObject("InsertCoin").name;
            UnityBrowserBridge.Instance.ExecuteJS(
                $"await InsertCoin({optionsJson}, '{onLaunchCallBack.GetMethodInfo().Name}', '{gameObjectName}')");
#endif
        }

        private static void MockOnPlayerJoinBrowser(Action<Player> onPlayerJoinCallback)
        {
            throw new NotImplementedException();
//             if (!OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback)) OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
//
//             var gameObjectName = GetGameObject("devManager").name;
//
// #if UNITY_EDITOR
//             UnityBrowserBridge.Instance.ExecuteJS($"OnPlayerJoin('{gameObjectName}')");
// #endif
        }

//         private static void MockOnPlayerQuitBrowser(Action<Player> onPlayerJoinCallback)
//         {
//             if (!OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback)) OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
//
//             var gameObjectName = GetGameObject("devManager").name;
//
// #if UNITY_EDITOR
//             UnityBrowserBridge.Instance.ExecuteJS($"OnPlayerQuit('{gameObjectName}')");
// #endif
//         }
//         
         // private static void MockOnPlayerQuitLocal(Action<string> onPlayerQuitCallback)
         // {
         //     Debug.Log("On Player Quit");
         //     var testPlayer = GetPlayer(PlayerId);
         //     testPlayer.OnQuitCallbacks.Add(onPlayerQuitCallback);
         //     __OnQuitInternalHandler(PlayerId);
         // }

        /// <summary>
        /// This function is used by GetPlayerID in PlayroomKitDevManager, GetPlayer is only invoked
        /// in mock mode by the JS bridge
        /// </summary>
        /// <param name="playerId"></param>
        public static void MockOnPlayerJoinWrapper(string playerId)
        {
            throw new NotImplementedException("MockOnPlayerJoinWrapper");
        }

        private static void MockSetStateBrowser(string key, object value, bool reliable)
        {
            var flag = reliable ? 1 : 0;

            string jsonString;
            if (value is int intValue)
            {
                jsonString = intValue.ToString();
            }
            else
            {
                jsonString = JsonUtility.ToJson(value);
            }

#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS($"SetState('{key}', '{jsonString}', {flag})");
#endif
        }


        private static T MockGetStateBrowser<T>(string key)
        {
#if UNITY_EDITOR
            string returnVal = UnityBrowserBridge.Instance.ExecuteJS<string>($"GetState('{key}')");
            // Dictionary to map types to parsing functions
            var typeParsers = new Dictionary<Type, Func<string, object>>()
            {
                { typeof(string), val => val },
                { typeof(Vector2), val => JsonUtility.FromJson<Vector2>(val) },
                { typeof(Vector3), val => JsonUtility.FromJson<Vector3>(val) },
                { typeof(Quaternion), val => JsonUtility.FromJson<Quaternion>(val) },
                { typeof(Color), val => JsonUtility.FromJson<Color>(val) },
                { typeof(int), val => int.Parse(val) },
                { typeof(float), val => float.Parse(val) },
                { typeof(bool), val => bool.Parse(val) }
            };

            if (typeParsers.TryGetValue(typeof(T), out var parser))
            {
                try
                {
                    return (T)parser(returnVal);
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException(
                        $"Error converting value of type '{typeof(string)}' to '{typeof(T)}': {ex.Message}");
                }
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type '{typeof(string)}' to '{typeof(T)}'.");
            }
#else
            return default;
#endif
        }

        private static void MockPlayerSetStateBrowser(string playerID, string key, object value, bool reliable = false)
        {
            var flag = reliable ? 1 : 0;


            string jsonString;
            if (value is int intValue)
            {
                jsonString = intValue.ToString();
            }
            else
            {
                jsonString = JsonUtility.ToJson(value);
            }


#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS(
                $"SetPlayerStateByPlayerId('{playerID}','{key}', {jsonString}, {flag})");
#endif
        }

        private static T MockPlayerGetStateBrowser<T>(string playerID, string key)
        {
#if UNITY_EDITOR


            var returnVal =
                UnityBrowserBridge.Instance.ExecuteJS<string>($"GetPlayerStateByPlayerId('{playerID}','{key}')");


            if (string.IsNullOrEmpty(returnVal))
            {
                throw new InvalidCastException(
                    $"Received null or empty string for playerID '{playerID}' or key '{key}'. Cannot convert to '{typeof(T)}'.");
            }


            // Dictionary to map types to parsing functions
            var typeParsers = new Dictionary<Type, Func<string, object>>()
            {
                { typeof(string), val => val },
                { typeof(Vector2), val => JsonUtility.FromJson<Vector2>(val) },
                { typeof(Vector3), val => JsonUtility.FromJson<Vector3>(val) },
                { typeof(Quaternion), val => JsonUtility.FromJson<Quaternion>(val) },
                { typeof(Color), val => JsonUtility.FromJson<Color>(val) },
                { typeof(int), val => int.Parse(val) },
                { typeof(float), val => float.Parse(val) },
                { typeof(bool), val => bool.Parse(val) }
            };

            if (typeParsers.TryGetValue(typeof(T), out var parser))
            {
                try
                {
                    return (T)parser(returnVal);
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException(
                        $"Error converting value of type '{typeof(string)}' to '{typeof(T)}': {ex.Message}");
                }
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type '{typeof(string)}' to '{typeof(T)}'.");
            }
#else
            return default;
#endif
        }

        private static string MockGetRoomCodeBrowser()
        {
#if UNITY_EDITOR
            return UnityBrowserBridge.Instance.ExecuteJS<string>($"GetRoomCode()");
#else
            return default;
#endif
        }

        private static Player MockMyPlayerBrowser()
        {
#if UNITY_EDITOR
            var id = UnityBrowserBridge.Instance.ExecuteJS<string>($"MyPlayer()");
            return GetPlayerById(id);
#else
            return default;
#endif
        }

        private static bool MockIsHostBrowser()
        {
#if UNITY_EDITOR
            return UnityBrowserBridge.Instance.ExecuteJS<bool>($"IsHost()");
#else
            return default;
#endif
        }

        private static bool MockIsStreamScreenBrowser()
        {
#if UNITY_EDITOR
            return UnityBrowserBridge.Instance.ExecuteJS<bool>($"IsStreamScreen()");
#else
            return default;
#endif
        }

        private static Player.Profile MockGetProfileBrowser(string playerID)
        {
#if UNITY_EDITOR
            string json = UnityBrowserBridge.Instance.ExecuteJS<string>($"GetProfile('{playerID}')");
            var profileData = Helpers.ParseProfile(json);
            return profileData;
#else
            return default;
#endif
        }

        private static void MockStartMatchmakingBrowser()
        {
#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS($"await StartMatchmaking()");
#endif
        }

        private static void MockKickBrowser(string playerID)
        {
#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS($"await Kick('{playerID}')");
#endif
        }

        private static void MockResetStatesBrowser(string[] keysToExclude, Action onKickCallback = null)
        {
#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS($"await ResetStates('{keysToExclude}', '{onKickCallback}')");
            onKickCallback?.Invoke();
#endif
        }


        private static void ResetPlayersStatesBrowser(string keysToExclude = null, Action onKickCallback = null)
        {
#if UNITY_EDITOR
            var gameObjectName = GetGameObject("PlayerJoin").name;
            UnityBrowserBridge.Instance.ExecuteJS(
                $"await ResetPlayersStates('{keysToExclude}', {onKickCallback})");
            onKickCallback?.Invoke();
#endif
        }

        private static void MockOnDisconnectBrowser(Action callback)
        {
#if UNITY_EDITOR
            string key = Guid.NewGuid().ToString();
            string callbackKey = $"OnDisconnect_{key}";
            GameObject callbackObject = new GameObject(callbackKey);
            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(callback, callbackKey);
            UnityBrowserBridge.Instance.ExecuteJS(
                $"OnDisconnect('{callbackKey}')");
#endif
        }

        private static void MockWaitForStateBrowser(string state, Action<string> callback)
        {
#if UNITY_EDITOR
            string callbackKey = $"WaitForState_{state}";
            GameObject callbackObject = new GameObject(callbackKey);

            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(callback, callbackKey);

            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");

            UnityBrowserBridge.Instance.ExecuteJS(
                $"WaitForState('{state}', '{callbackKey}')");
#endif
        }

        private static void MockRpcRegisterBrowser(string name, Action<string, string> callback,
            string onResponseReturn = null)
        {
#if UNITY_EDITOR

            string callbackKey = $"RpcEvent_{name}";
            GameObject callbackObject = new GameObject(callbackKey);

            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(callback, callbackKey);

            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");

            UnityBrowserBridge.Instance.ExecuteJS(
                $"RpcRegister('{name}', '{callbackKey}')");
#endif
        }

        private static void MockRpcCallBrowser(string name, object data, RpcMode mode, Action callbackOnResponse = null)
        {
#if UNITY_EDITOR

            UnityBrowserBridge.Instance.ExecuteJS($"RpcCall('{name}', '{data}', '{mode}')");
#endif
        }
    }
}