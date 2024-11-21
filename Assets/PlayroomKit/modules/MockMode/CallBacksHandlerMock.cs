using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Playroom
{
    public class CallBacksHandlerMock : MonoBehaviour
    {
        private readonly Dictionary<string, (GameObject gameObject, string methodName)> callbacks = new();

        [SerializeField]
        private PlayroomkitDevManager manager;

        private static CallBacksHandlerMock _instance;

        public static CallBacksHandlerMock Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CallBacksHandlerMock>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("CallbackManager");
                        _instance = go.AddComponent<CallBacksHandlerMock>();
                        DontDestroyOnLoad(go);
                    }
                }

                return _instance;
            }
        }

        private void Start()
        {
            gameObject.transform.SetParent(manager.gameObject.transform);
        }


        public void RegisterCallbackObject(string key, GameObject gameObject, string methodName)
        {
            if (!callbacks.ContainsKey(key))
            {
                callbacks.TryAdd(key, (gameObject, methodName));
            }
        }

        public void HandleRPC(string jsonData)
        {
            var jsonNode = JSON.Parse(jsonData);

            string key = jsonNode["key"];

            string returnData = jsonNode["parameter"]["data"];
            string callerId = jsonNode["parameter"]["callerId"];

            if (callbacks.TryGetValue(key, out var callbackInfo))
            {
                Debug.LogWarning(
                    $"key: {key}, gameObjectName: {callbackInfo.gameObject.name}, callbackName: {callbackInfo.methodName}");

                callbackInfo.gameObject.SendMessage(callbackInfo.methodName, new string[] { returnData, callerId },
                    SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning($"No callback registered for key: {key}");
            }
        }

        // Called from Browser JS
        public void InvokeCallback(string jsonData)
        {
            var jsonNode = JSON.Parse(jsonData);

            string key = jsonNode["key"];
            string parameter = jsonNode["parameter"];

            if (callbacks.TryGetValue(key, out var callbackInfo))
            {
                DebugLogger.LogWarning(
                    $"key: {key}, gameObjectName: {callbackInfo.gameObject.name}, callbackName: {callbackInfo.methodName}");

                callbackInfo.gameObject.SendMessage(callbackInfo.methodName, parameter,
                    SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning($"No callback registered for key: {key}");
            }
        }
    }

    [System.Serializable]
    public class RPCData
    {
        public string data;
        public string callerId;
    }
}