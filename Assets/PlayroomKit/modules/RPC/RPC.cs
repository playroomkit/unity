using System;
using System.Collections;
using System.Collections.Generic;
using AOT;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class RPC : IRPC
        {
            private static PlayroomKit _playroomKit;
            private readonly IInterop _interop;

            private static List<string> RpcCalledEvents = new();
            private static Dictionary<string, List<Action>> OnResponseCallbacks = new();

            #region Constructors

            public RPC(PlayroomKit playroomKit)
            {
                _playroomKit = playroomKit;
                _interop = new PlayroomKitInterop();
            }

            public RPC(PlayroomKit playroomKit, IInterop interop)
            {
                _playroomKit = playroomKit;
                _interop = interop;
            }

            #endregion

            public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null)
            {
                CallbackManager.RegisterRpcCallback(rpcRegisterCallback, name);
                _interop.RpcRegisterWrapper(name, InvokeRpcRegisterCallBack, onResponseReturn);
            }

            public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
            {
                string jsonData = ConvertToJson(data);

                if (OnResponseCallbacks.ContainsKey(name))
                {
                    OnResponseCallbacks[name].Add(callbackOnResponse);
                }
                else
                {
                    OnResponseCallbacks.Add(name, new List<Action> { callbackOnResponse });
                    if (!RpcCalledEvents.Contains(name))
                    {
                        RpcCalledEvents.Add(name);
                    }
                }

                JSONArray jsonArray = new JSONArray();
                foreach (string item in RpcCalledEvents)
                {
                    jsonArray.Add(item);
                }

                string jsonString = jsonArray.ToString();

/*
                    This is required to sync the rpc events between all players, without this players won't know which event has been called.
                    Update: This fix works fine for now, but there might be a better way.
                    this is a temporary fix, RPCs need to be handled within JSLIB for better control.
*/
                _playroomKit.SetState("rpcCalledEventName", jsonString, reliable: true);
                _interop.RpcCallWrapper(name, jsonData, mode, InvokeOnResponseCallback);
            }

            public void RpcCall(string name, object data, Action callbackOnResponse = null)
            {
                RpcCall(name, data, RpcMode.ALL, callbackOnResponse);
            }

            #region Invokers

            [MonoPInvokeCallback(typeof(Action))]
            protected static void InvokeOnResponseCallback()
            {
                var namesToRemove = new List<string>();

                foreach (string name in RpcCalledEvents)
                {
                    try
                    {
                        if (OnResponseCallbacks.TryGetValue(name, out List<Action> callbacks))
                        {
                            foreach (var callback in callbacks)
                            {
                                callback?.Invoke();
                            }

                            namesToRemove.Add(name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"C#: Error in Invoking callback for RPC event name: '{name}': {ex.Message}");
                    }
                }

                foreach (var name in namesToRemove)
                {
                    RpcCalledEvents.Remove(name);
                    OnResponseCallbacks.Remove(name);
                }
            }

            [MonoPInvokeCallback(typeof(Action<string, string>))]
            protected static void InvokeRpcRegisterCallBack(string dataJson, string senderJson)
            {
                try
                {
                    if (!Players.ContainsKey(senderJson))
                    {
                        var player = new Player(senderJson, new Player.PlayerService(senderJson));
                        Players.Add(senderJson, player);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                List<string> updatedRpcCalledEvents = new();
                // This state is required to update the called rpc events list, (Temp fix see RpcCall for more) 
                string nameJson = _playroomKit.GetState<string>("rpcCalledEventName");

                JSONArray jsonArray = JSON.Parse(nameJson).AsArray;
                foreach (JSONNode node in jsonArray)
                {
                    string item = node.Value;
                    updatedRpcCalledEvents.Add(item);
                }
                
                for (var i = 0; i < updatedRpcCalledEvents.Count; i++)
                {
                    string name = updatedRpcCalledEvents[i];
                    CallbackManager.InvokeRpcRegisterCallBack(name, dataJson, senderJson);
                }
            }

            #endregion

            #region Helpers

            private static string ConvertToJson(object data)
            {
                if (data == null)
                {
                    return null;
                }
                else if (data.GetType().IsPrimitive || data is string)
                {
                    return data.ToString();
                }

                if (data is Vector2 vector2)
                {
                    return JsonUtility.ToJson(vector2);
                }
                else if (data is Vector3 vector3)
                {
                    return JsonUtility.ToJson(vector3);
                }
                else if (data is Vector4 vector4)
                {
                    return JsonUtility.ToJson(vector4);
                }
                else if (data is Quaternion quaternion)
                {
                    return JsonUtility.ToJson(quaternion);
                }
                else
                {
                    return ConvertComplexToJson(data);
                }
            }

            private static string ConvertComplexToJson(object data)
            {
                if (data is IDictionary dictionary)
                {
                    JSONObject dictNode = new JSONObject();
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        dictNode[entry.Key.ToString()] = ConvertToJson(entry.Value);
                    }

                    return dictNode.ToString();
                }
                else if (data is IEnumerable enumerable)
                {
                    JSONArray arrayNode = new JSONArray();
                    foreach (object element in enumerable)
                    {
                        arrayNode.Add(ConvertToJson(element));
                    }

                    return arrayNode.ToString();
                }
                else
                {
                    Debug.Log($"{data} is '{data.GetType()}' which is currently not supported by RPC!");
                    return JSON.Parse("{}").ToString();
                }
            }

            #endregion
        }
    }
}