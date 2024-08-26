using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Playroom
{
    /// <summary>
    /// This file contains the (old) editor only mock-mode which (tries) to simulate Playroom's functionality.
    /// </summary>
    public partial class PlayroomKit
    {
#if UNITY_EDITOR
        private const string PlayerId = "mockplayerID123";
        private static bool mockIsStreamMode;

        ///<summary> 
        /// This is private, instead of public, to prevent tampering in Mock Mode.
        /// Reason: In Mock Mode, only a single player can be tested. 
        /// Ref: https://docs.joinplayroom.com/usage/unity#mock-mode
        ///</summary>
        private static Dictionary<string, object> MockDictionary = new();

        private static void MockInsertCoinSimulated(InitOptions options, Action onLaunchCallBack)
        {
            isPlayRoomInitialized = true;
            Debug.Log("Coin Inserted");
            string optionsJson = null;
            if (options != null) optionsJson = SerializeInitOptions(options);
            onLaunchCallBack?.Invoke();
        }

        private static void MockOnPlayerJoinSimulated(Action<Player> onPlayerJoinCallback)
        {
            Debug.Log("On Player Join");
            var testPlayer = GetPlayer(PlayerId);
            OnPlayerJoinCallbacks.Add(onPlayerJoinCallback);
            __OnPlayerJoinCallbackHandler(PlayerId);
        }

        private static void MockSetStateSimulated(string key, object value)
        {
            if (MockDictionary.ContainsKey(key))
                MockDictionary[key] = value;
            else
                MockDictionary.Add(key, value);
        }


        private static T MockGetStateSimulated<T>(string key)
        {
            if (MockDictionary.TryGetValue(key, out var value) && value is T typedValue)
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

        public static void MockRpcRegister(string name, Action<string, string> rpcRegisterCallback,
            string onResponseReturn = null)
        {
            mockRegisterCallbacks.TryAdd(name, (rpcRegisterCallback, onResponseReturn));
        }


        private static void MockRpcCall(string name, object data, RpcMode mode, Action callbackOnResponse)
        {
            mockResponseCallbacks.TryAdd(name, callbackOnResponse);

            string stringData = Convert.ToString(data);
            var player = MyPlayer();

            if (mockRegisterCallbacks.TryGetValue(name, out var responseHandler))
            {
                responseHandler.callback?.Invoke(stringData, player.id);

                if (!string.IsNullOrEmpty(responseHandler.response))
                {
                    Debug.Log($"Response received: {responseHandler.response}");
                }
            }

            if (mockResponseCallbacks.TryGetValue(name, out var callback))
            {
                callback?.Invoke();
            }
        }
#endif
    }
}