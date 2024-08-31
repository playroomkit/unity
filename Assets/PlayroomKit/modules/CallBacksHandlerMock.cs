using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Playroom
{
    public class CallBacksHandlerMock : MonoBehaviour
    {
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
            var manager = FindObjectOfType<PlayroomkitDevManager>();
            gameObject.transform.SetParent(manager.gameObject.transform);
        }

        private Dictionary<string, (GameObject gameObject, string methodName)> callbacks = new();

        public void RegisterCallback(string key, GameObject gameObject, string methodName)
        {
            callbacks[key] = (gameObject, methodName);
        }

        public void InvokeCallback(string jsonData)
        {
            Debug.LogWarning($"jsonData: {jsonData}");

            var jsonNode = JSON.Parse(jsonData);

            string key = jsonNode["key"];
            string parameter = jsonNode["parameter"];


            if (callbacks.TryGetValue(key, out var callbackInfo))
            {
                Debug.LogWarning(
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
}