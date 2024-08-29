using System;
using System.Collections.Generic;
using System.Reflection;
using UBB;
using UnityEngine;
using UnityEngine.Networking;

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


#if UNITY_EDITOR

        private static void MockInsertCoinBrowser(InitOptions options, Action onLaunchCallBack)
        {
            isPlayRoomInitialized = true;

            Debug.Log("Coin Inserted!");

            string optionsJson = null;
            if (options != null) optionsJson = SerializeInitOptions(options);

            var gameObjectName = GetGameObject("InsertCoin").name;

            UnityBrowserBridge.Instance.ExecuteJS(
                $"await InsertCoin({optionsJson}, '{onLaunchCallBack.GetMethodInfo().Name}', '{gameObjectName}')");
        }

        private static void MockOnPlayerJoinBrowser(Action<Player> onPlayerJoinCallback)
        {
            if (!OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback)) OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);

            var gameObjectName = GetGameObject("PlayerJoin").name;

            UnityBrowserBridge.Instance.ExecuteJS($"OnPlayerJoin('{gameObjectName}')");
        }

        private static void MockSetStateBrowser(string key, object value, bool reliable)
        {
            var flag = reliable ? 1 : 0;

            UnityBrowserBridge.Instance.ExecuteJS($"SetState('{key}', {value}, {flag})");
        }


        private static T MockGetStateBrowser<T>(string key)
        {
            return UnityBrowserBridge.Instance.ExecuteJS<T>($"GetState('{key}')");
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


            UnityBrowserBridge.Instance.ExecuteJS(
                $"SetPlayerStateByPlayerId('{playerID}','{key}', {jsonString}, {flag})");
        }

        private static T MockPlayerGetStateBrowser<T>(string playerID, string key)
        {
            var returnVal =
                UnityBrowserBridge.Instance.ExecuteJS<string>($"GetPlayerStateByPlayerId('{playerID}','{key}')");

            // Handle types with direct deserialization
            if (typeof(T) == typeof(string))
            {
                return (T)(object)returnVal;
            }

            if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3))
            {
                try
                {
                    if (returnVal != null)
                    {
                        return (T)(object)JsonUtility.FromJson<Vector3>(returnVal);
                    }
                }
                catch
                {
                    throw new InvalidCastException($"Cannot parse JSON to type '{typeof(T)}'.");
                }
            }


            try
            {
                if (typeof(T) == typeof(int))
                {
                    return (T)(object)int.Parse(returnVal);
                }

                if (typeof(T) == typeof(float))
                {
                    return (T)(object)float.Parse(returnVal);
                }

                if (typeof(T) == typeof(bool))
                {
                    return (T)(object)bool.Parse(returnVal);
                }

                if (typeof(T) == typeof(Color))
                {
                    return (T)(object)JsonUtility.FromJson<Color>(returnVal);
                }

                throw new InvalidCastException($"Cannot convert value of type '{typeof(string)}' to '{typeof(T)}'.");
            }
            catch (Exception ex)
            {
                // Handle conversion failure
                throw new InvalidCastException(
                    $"Error converting value of type '{typeof(string)}' to '{typeof(T)}': {ex.Message}");
            }
        }

        private static string MockGetRoomCodeBrowser()
        {
            return UnityBrowserBridge.Instance.ExecuteJS<string>($"GetRoomCode()");
        }

        private static Player MockMyPlayerBrowser()
        {
            var id = UnityBrowserBridge.Instance.ExecuteJS<string>($"MyPlayer()");
            return GetPlayer(id);
        }

        private static bool MockIsHostBrowser()
        {
            return UnityBrowserBridge.Instance.ExecuteJS<bool>($"IsHost()");
        }

        private static bool MockIsStreamScreenBrowser()
        {
            return UnityBrowserBridge.Instance.ExecuteJS<bool>($"IsStreamScreen()");
        }

        private static Player.Profile MockGetProfileBrowser(string playerID)
        {
            string json = UnityBrowserBridge.Instance.ExecuteJS<string>($"GetProfile('{playerID}')");

            Debug.Log(json);

            var profileData = ParseProfile(json);
            return profileData;
        }

        private static void MockStartMatchmakingBrowser()
        {
            UnityBrowserBridge.Instance.ExecuteJS($"await StartMatchmaking()");
        }

        private static void MockKickBrowser(string playerID)
        {
            UnityBrowserBridge.Instance.ExecuteJS($"await Kick('{playerID}')");
        }

        private static void MockResetStatesBrowser(string[] keysToExclude, Action onKickCallback = null)
        {
            UnityBrowserBridge.Instance.ExecuteJS($"await ResetStates('{keysToExclude}')");
        }

        private static void ResetPlayersStatesBrowser(Action onKickCallback = null, string keysToExclude = null)
        {
            UnityBrowserBridge.Instance.ExecuteJS($"await ResetPlayersStates('{keysToExclude}')");
            onKickCallback?.Invoke();
        }
#endif
    }
}