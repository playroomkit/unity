using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Playroom
{
    public static class CallbackManager
    {
        private static Dictionary<string, Delegate> callbacks = new();


        [DllImport("__Internal")]
        private static extern string GetKey(string key);

        [DllImport("__Internal")]
        private static extern void SendKey(string key);


        public static void RegisterCallback(Action callback)
        {
            var key = GenerateKey();
            SendKey(key);
            if (callbacks.ContainsKey(key))
            {
                callbacks[key] = callback;
            }
            else
            {
                callbacks.Add(key, callback);
            }
        }

        public static void InvokeCallback()
        {
            string mappedKey = "";
            foreach (var kvp in callbacks)
            {
                mappedKey = GetKey(kvp.Key);
                Debug.Log($"Key: {kvp.Key} Mapped Key: {mappedKey}");
                break;
            }

            if (callbacks.TryGetValue(mappedKey, out Delegate callback))
            {
                (callback as Action)?.Invoke();


            }
            else
            {
                Debug.LogError($"Callback with key {mappedKey} not found!");
            }



        }

        private static string GenerateKey()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
