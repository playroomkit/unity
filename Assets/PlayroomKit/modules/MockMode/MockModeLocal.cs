using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Playroom
{
    /// <summary>
    /// This file contains the (old) editor only mock-mode which (tries) to simulate Playroom's functionality.
    /// </summary>
    public partial class PlayroomKit
    {
        private const string PlayerId = "mockplayerID123";
        private static bool mockIsStreamMode;

        ///<summary> 
        /// This is private, instead of public, to prevent tampering in Mock Mode.
        /// Reason: In Mock Mode, only a single player can be tested. 
        /// Ref: https://docs.joinplayroom.com/usage/unity#mock-mode
        ///</summary>
        private static Dictionary<string, object> mockGlobalStatesDictionary = new();

        private static Dictionary<string, object> mockPlayerStatesDictionary = new();

        private static void MockInsertCoinLocal(InitOptions options, Action onLaunchCallBack)
        {
            isPlayRoomInitialized = true;
            Debug.Log("Coin Inserted");
            string optionsJson = null;
            if (options != null) optionsJson = SerializeInitOptions(options);
            onLaunchCallBack?.Invoke();
        }

        private static void MockOnPlayerJoinLocal(Action<Player> onPlayerJoinCallback)
        {
            // Debug.Log("On Player Join");
            // var testPlayer = GetPlayer(PlayerId);
            // OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
            throw new NotImplementedException();
        }

        private static void MockSetStateLocal(string key, object value)
        {
            if (mockGlobalStatesDictionary.ContainsKey(key))
                mockGlobalStatesDictionary[key] = value;
            else
                mockGlobalStatesDictionary.Add(key, value);
        }

        private static void MockPlayerSetStateLocal(string key, object value)
        {
            if (mockPlayerStatesDictionary.ContainsKey(key))
                mockPlayerStatesDictionary[key] = value;
            else
                mockPlayerStatesDictionary.Add(key, value);
        }


        private static T MockGetStateLocal<T>(string key)
        {
            if (mockGlobalStatesDictionary.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            else
            {
                Debug.LogWarning($"No {key} in States or value is not of type {typeof(T)}");
                return default;
            }
        }

        private static T MockPlayerGetStateLocal<T>(string key)
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


        private static Dictionary<string, (Action<string, string> callback, string response)> mockRegisterCallbacks =
            new();

        private static Dictionary<string, Action> mockResponseCallbacks = new();

        private static void MockRpcRegisterLocal(string name, Action<string, string> rpcRegisterCallback,
            string onResponseReturn = null)
        {
            mockRegisterCallbacks.TryAdd(name, (rpcRegisterCallback, onResponseReturn));
        }


        // private static void MockRpcCallLocal(string name, object data, RpcMode mode, Action callbackOnResponse = null)
        // {
        //     mockResponseCallbacks.TryAdd(name, callbackOnResponse);
        //
        //     string stringData = Convert.ToString(data);
        //     var player = _MyPlayer();
        //
        //     if (mockRegisterCallbacks.TryGetValue(name, out var responseHandler))
        //     {
        //         responseHandler.callback?.Invoke(stringData, player.id);
        //
        //         if (!string.IsNullOrEmpty(responseHandler.response))
        //         {
        //             Debug.Log($"Response received: {responseHandler.response}");
        //         }
        //     }
        //
        //     if (mockResponseCallbacks.TryGetValue(name, out var callback))
        //     {
        //         callback?.Invoke();
        //     }
        // }

        // TODO: need to reimplement when local co-op is added
        private static string MockGetRoomCodeLocal()
        {
            return "mock123";
        }

        private static Player MockMyPlayerLocal()
        {
            return GetPlayer(PlayerId);
        }

        private static bool MockIsHostLocal()
        {
            return true;
        }

        // TODO: need to reimplement when local co-op is added
        private static bool MockIsStreamScreenLocal()
        {
            return mockIsStreamMode;
        }

        private static Player.Profile MockGetProfileLocal()
        {
            PlayroomKit.Player.Profile.PlayerProfileColor mockPlayerProfileColor = new()
            {
                r = 166,
                g = 0,
                b = 142,
                hexString = "#a6008e"
            };
            ColorUtility.TryParseHtmlString(mockPlayerProfileColor.hexString, out UnityEngine.Color color1);
            var testProfile = new Player.Profile()
            {
                color = color1,
                name = "MockPlayer",
                playerProfileColor = mockPlayerProfileColor,
                photo = "testPhoto"
            };
            return testProfile;
        }

        private static void MockStartMatchmakingLocal()
        {
            Debug.Log("Matchmaking doesn't work in local mock mode!");
        }

        private static void MockKickLocal(Action onKickCallback)
        {
            var player = GetPlayer(PlayerId);
            Players.Remove(player.id);
            onKickCallback?.Invoke();
        }

        // TODO: needs to be reimplemented for local co op mode.
        private static void MockResetStatesLocal(string[] keysToExclude, Action onKickCallback)
        {
            List<string> keysToRemove =
                mockGlobalStatesDictionary.Keys.Where(key => !keysToExclude.Contains(key)).ToList();
            foreach (string key in keysToRemove) mockGlobalStatesDictionary.Remove(key);
            onKickCallback?.Invoke();
        }

        private static void MockResetPlayersStatesLocal(string[] keysToExclude = null, Action onKickCallback = null)
        {
            if (keysToExclude == null || keysToExclude.Length == 0)
            {
                keysToExclude = Array.Empty<string>();
            }

            List<string> keysToRemove =
                mockPlayerStatesDictionary.Keys.Where(key => !keysToExclude.Contains(key)).ToList();

            foreach (string key in keysToRemove)
            {
                mockPlayerStatesDictionary.Remove(key);
            }

            onKickCallback?.Invoke();
        }

        private static void MockOnDisconnectLocal(Action callback)
        {
            callback?.Invoke();
        }

        private static void MockWaitForStateLocal(string key, Action<string> callback)
        {
            Debug.Log("Wait for state is not supported in local mode yet!");
        }
    }
}