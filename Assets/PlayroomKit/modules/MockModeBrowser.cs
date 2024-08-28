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


        private static void MockInsertCoinBrowser(InitOptions options, Action onLaunchCallBack)
        {
            isPlayRoomInitialized = true;

            Debug.Log("Coin Inserted!");

            string optionsJson = null;
            if (options != null) optionsJson = SerializeInitOptions(options);

            var gameObjectName = GetGameObject("InsertCoin").name;
#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS(
                $"await InsertCoin({optionsJson}, '{onLaunchCallBack.GetMethodInfo().Name}', '{gameObjectName}')");
#endif
        }

        private static void MockOnPlayerJoinBrowser(Action<Player> onPlayerJoinCallback)
        {
            if (!OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback)) OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);

            var gameObjectName = GetGameObject("PlayerJoin").name;
#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS($"OnPlayerJoin('{gameObjectName}')");
#endif
        }

        private static void MockSetStateBrowser(string key, object value, bool reliable)
        {
            var flag = reliable ? 1 : 0;
#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS($"SetState('{key}', {value}, {flag})");
#endif
        }


#if UNITY_EDITOR
        private static T MockGetStateBrowser<T>(string key)
        {
            return UnityBrowserBridge.Instance.ExecuteJS<T>($"GetState('{key}')");
        }

        private static void MockPlayerSetStateBrowser(string playerID, string key, object value, bool reliable = false)
        {
            var flag = reliable ? 1 : 0;

#if UNITY_EDITOR

            string jsonString = JsonUtility.ToJson(value);


            UnityBrowserBridge.Instance.ExecuteJS(
                $"SetPlayerStateByPlayerId('{playerID}','{key}', {jsonString}, {flag})");
#endif
        }


        private static T MockPlayerGetStateBrowser<T>(string playerID, string key)
        {
            var returnVal =
                UnityBrowserBridge.Instance.ExecuteJS<string>($"GetPlayerStateByPlayerId('{playerID}','{key}')");


            if (typeof(T) == typeof(string))
            {
                return (T)(object)returnVal;
            }

            if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3))
            {
                try
                {
                    var parsedObject = JsonUtility.FromJson<T>(returnVal);

                    return parsedObject;
                }
                catch
                {
                    // Handle JSON parsing failure
                    throw new InvalidCastException($"Cannot parse JSON to type '{typeof(T)}'.");
                }
            }

            // Handle other types
            try
            {
                T a = (T)Convert.ChangeType(returnVal, typeof(T));
                Debug.Log(a.GetType());
                return a;
            }
            catch
            {
                // Handle conversion failure
                throw new InvalidCastException($"Cannot convert value of type '{typeof(string)}' to '{typeof(T)}'.");
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

#endif
    }
}