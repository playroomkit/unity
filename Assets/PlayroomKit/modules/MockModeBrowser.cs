using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UBB;
using Unity.VisualScripting;
using UnityEngine;

namespace Playroom
{
    /// <summary>
    /// This file contains the new  mock-mode which uses browser-driver to run Playroom's within the editor!.
    /// </summary>
    public partial class PlayroomKit
    {
#if UNITY_EDITOR
        // Store GameObjects by function type
        private static Dictionary<string, GameObject> gameObjectReferences = new Dictionary<string, GameObject>();

        public static void RegisterGameObject(string key, GameObject gameObject)
        {
            if (!gameObjectReferences.ContainsKey(key))
            {
                gameObjectReferences.Add(key, gameObject);
            }
            else
            {
                gameObjectReferences[key] = gameObject;
            }
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

            string gameObjectName = GetGameObject("InsertCoin").name;

            UnityBrowserBridge.Instance.ExecuteJS(
                $"await InsertCoin({optionsJson}, '{onLaunchCallBack.GetMethodInfo().Name}', '{gameObjectName}')");
        }

        private static void MockOnPlayerJoinBrowser(Action<Player> onPlayerJoinCallback)
        {
            if (!OnPlayerJoinCallbacks.Contains(onPlayerJoinCallback))
            {
                OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
            }

            var gameObjectName = GetGameObject("PlayerJoin").name;

            UnityBrowserBridge.Instance.ExecuteJS($"OnPlayerJoin('{gameObjectName}')");
        }

        private static void MockSetStateBrowser(string key, object value, bool reliable)
        {
            var flag = reliable ? 1 : 0;
            UnityBrowserBridge.Instance.ExecuteJS($"SetState('{key}', {value}, {flag})");
        }

        private static void MockPlayerSetStateBrowser(string playerID, string key, object value, bool reliable = false)
        {
            var flag = reliable ? 1 : 0;
            UnityBrowserBridge.Instance.ExecuteJS(
                $"SetPlayerStateByPlayerId('{playerID}','{key}', {value}, {reliable})");
        }

        private static T MockGetStateBrowser<T>(string key)
        {
            return UnityBrowserBridge.Instance.ExecuteJS<T>($"GetState('{key}')");
        }

        private static T MockPlayerGetStateBrowser<T>(string playerID, string key)
        {
            return UnityBrowserBridge.Instance.ExecuteJS<T>($"GetPlayerStateByPlayerId('{playerID}','{key}')");
        }
#endif
    }
}