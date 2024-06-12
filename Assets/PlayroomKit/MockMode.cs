using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        private const string PlayerId = "mockplayerID123";
        private static bool mockIsStreamMode;

        ///<summary> 
        /// This is private, instead of public, to prevent tampering in Mock Mode.
        /// Reason: In Mock Mode, only a single player can be tested. 
        /// Ref: https://docs.joinplayroom.com/usage/unity#mock-mode
        ///</summary>
        private static Dictionary<string, object> MockDictionary = new();




        private static void MockInsertCoin(InitOptions options, Action onLaunchCallBack)
        {
            isPlayRoomInitialized = true;

            Debug.Log("Coin Inserted");

            string optionsJson = null;
            if (options != null) optionsJson = SerializeInitOptions(options);
            onLaunchCallBack?.Invoke();
        }

        private static void MockSetState(string key, object value)
        {
            if (MockDictionary.ContainsKey(key))
                MockDictionary[key] = value;
            else
                MockDictionary.Add(key, value);
        }

        private static T MockGetState<T>(string key)
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

        private static Dictionary<string, Action> mockCallbackOnResponse = new();

        private static void MockRpcRegister(string name, Action<string, string> rpcRegisterCallback, string onResponseReturn = null)
        {
            CallbackManager.RegisterCallback(rpcRegisterCallback, name);
            if (!string.IsNullOrEmpty(onResponseReturn)) Debug.Log(onResponseReturn);
        }

        private static void MockRpcCall(string name, object data, RpcMode mode, Action callbackOnResponse)
        {
            if (!mockCallbackOnResponse.ContainsKey(name))
                mockCallbackOnResponse.Add(name, callbackOnResponse);

            string stringData = Convert.ToString(data);
            var player = MyPlayer();
            CallbackManager.InvokeCallback(name, stringData, player.id);

            if (mockCallbackOnResponse.TryGetValue(name, out var callback))
            {
                callback?.Invoke();
            }
        }
    }

}