using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Playroom
{
    public static class CallbackManager
    {
        private static Dictionary<string, Delegate> callbacks = new();
        private static readonly HashSet<string> RegisteredEvents = new();

        private static Dictionary<string, List<Action<string, string>>> RpcCallBacks = new();

        public static void RegisterRpcCallback(Action<string, string> callback, string key = null)
        {
            if (string.IsNullOrEmpty(key))
                key = GenerateKey();

            if (RpcCallBacks.ContainsKey(key))
            {
                RpcCallBacks[key].Add(callback);
            }
            else
            {
                RpcCallBacks.Add(key, new List<Action<string, string>> { callback });
            }
        }

        public static void InvokeRpcRegisterCallBack(string name, string data, string sender)
        {
            if (RpcCallBacks.TryGetValue(name, out List<Action<string, string>> callbacks))
            {
                for (var i = 0; i < callbacks.Count; i++)
                {
                    Action<string, string> callback = callbacks[i];
                    callback?.Invoke(data, sender);
                }
            }
            else
            {
                Debug.LogWarning(
                    $"Callback with key {name} not found!, maybe register the callback or call the correct playroom function?");
            }
        }

        public static bool IsEventRegistered(string eventName)
        {
            return RegisteredEvents.Contains(eventName);
        }

        public static void MarkEventAsRegistered(string eventName)
        {
            RegisteredEvents.Add(eventName);
        }

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
                DebugLogger.Log(
                    $"Callback with key {key} not found!, maybe register the callback or call the correct playroom function?");
            }
        }

        public static void InvokeCallback(string key, TurnData turnData)
        {
            if (callbacks.TryGetValue(key, out Delegate callback))
            {
                if (callback is Action<TurnData> action) action?.Invoke(turnData);
                else
                    Debug.LogError(
                        $"Callback with key {key} is of unsupported type or incorrect number of arguments: {turnData}!");
            }
        }

        public static void InvokeCallback(string key, List<TurnData> turnData)
        {
            if (callbacks.TryGetValue(key, out Delegate callback))
            {
                if (callback is Action<List<TurnData>> action) action?.Invoke(turnData);
                else
                    Debug.LogError(
                        $"Callback with key {key} is of unsupported type or incorrect number of arguments: {turnData}!");
            }
        }

        public static bool CheckCallback(string key)
        {
            return callbacks.TryGetValue(key, out Delegate callback);
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