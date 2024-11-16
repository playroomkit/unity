using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public enum RpcMode
        {
            ALL,
            OTHERS,
            HOST
        }

        public interface IRPC
        {
            protected static List<string> rpcCalledEvents = new();

            protected static Dictionary<string, List<Action>> OnResponseCallbacks =
                new Dictionary<string, List<Action>>();

            public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null);

            public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null);

            // Default Mode
            public void RpcCall(string name, object data, Action callbackOnResponse = null);

            [MonoPInvokeCallback(typeof(Action))]
            protected static void InvokeOnResponseCallback()
            {
                var namesToRemove = new List<string>();

                foreach (string name in rpcCalledEvents)
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
                    rpcCalledEvents.Remove(name);
                    OnResponseCallbacks.Remove(name);
                }
            }


            protected static string ConvertToJson(object data)
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

            protected static string ConvertComplexToJson(object data)
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

                //Fix this later
                PlayroomKit _playroomKit = new PlayroomKit();
                string nameJson = _playroomKit.GetState<string>("rpcCalledEventName");
                //

                Debug.Log($"rpcCalledEventName {nameJson}: {dataJson}");

                JSONArray jsonArray = JSON.Parse(nameJson).AsArray;
                foreach (JSONNode node in jsonArray)
                {
                    string item = node.Value;
                    updatedRpcCalledEvents.Add(item);
                }

                foreach (string name in updatedRpcCalledEvents)
                {
                    CallbackManager.InvokeCallback(name, dataJson, senderJson);
                }
            }
        }
    }
}