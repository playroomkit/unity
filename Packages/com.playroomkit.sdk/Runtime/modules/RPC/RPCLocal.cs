using UnityEngine;
using System.Collections.Generic;
using System;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class RPCLocal : IRPC
        {
            private static Dictionary<string, (Action<string, string> callback, string response)>
                mockRegisterCallbacks =
                    new();

            private static Dictionary<string, Action> mockResponseCallbacks = new();

            private readonly IInterop _interop;

            public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null)
            {
                mockRegisterCallbacks.TryAdd(name, (rpcRegisterCallback, onResponseReturn));
            }

            public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
            {
                if (mockRegisterCallbacks.ContainsKey(name))
                {
                    mockResponseCallbacks.TryAdd(name, callbackOnResponse);
                }
                else
                {
                    Debug.LogWarning("Already registered callback for this event");
                }

                string stringData = Convert.ToString(data);
                var player = GetPlayerById("mockplayerID123");

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

            public void RpcCall(string name, object data, Action callbackOnResponse = null)
            {
                RpcCall(name, data, RpcMode.ALL, callbackOnResponse);
            }
        }
    }
}