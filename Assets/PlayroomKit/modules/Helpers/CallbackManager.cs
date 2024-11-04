using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Playroom
{
    public static class CallbackManager
    {
        private static Dictionary<string, Delegate> callbacks = new();

        public static string RegisterCallback(Delegate callback, string key = null)
        {
            if (string.IsNullOrEmpty(key))
                key = GenerateKey();

            if (!callbacks.ContainsKey(key))
            {
                callbacks.Add(key, callback);
            }
            else
            {
                callbacks[key] = callback;
            }

            return key;
        }

        public static void InvokeCallback(string key, params string[] args)
        {
           
            if (callbacks.TryGetValue(key, out Delegate callback))
            {
                if (callback is Action action && args.Length == 0) action?.Invoke();
                else if (callback is Action<string> stringAction && args.Length == 1) stringAction?.Invoke(args[0]);
                else if (callback is Action<string, string> doubleStringAction && args.Length == 2)
                    doubleStringAction?.Invoke(args[0], args[1]);
                else
                    Debug.LogError(
                        $"Callback with key {key} is of unsupported type or incorrect number of arguments: {args[0]}!");
            }
            else
            {
                Debug.LogWarning($"Callback with key {key} not found!, maybe register the callback or call the correct playroom function?");
            }
        }

        private static string GenerateKey()
        {
            return Guid.NewGuid().ToString();
        }


        /// <summary>
        /// Calls an external method to convert a string associated with a given key, called from the JS side only.
        /// </summary>
        /// <param name="key">The key associated with the string to be converted.</param>
        [DllImport("__Internal")]
        private static extern void ConvertString(string key);
    }
}