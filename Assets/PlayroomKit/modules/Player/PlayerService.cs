using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Playroom
{
    public class PlayerService : IPlayerBase
    {
        private static Dictionary<string, object> mockPlayerStatesDictionary = new();
        
        public void SetState(string id, string key, object value, bool reliable = false)
        {
            string jsonString = JsonUtility.ToJson(value);
            // Debug.Log(jsonString);
            SetPlayerStateStringById(id, key, jsonString, reliable);
        }

        public T GetState<T>(string id, string key)
        {
            Type type = typeof(T);
            if (type == typeof(int)) return (T)(object)GetPlayerStateIntById(id, key);
            else if (type == typeof(float)) return (T)(object)GetPlayerStateFloatById(id, key);
            else if (type == typeof(bool)) return (T)(object)GetPlayerStateBoolById(id, key);
            else if (type == typeof(string)) return (T)(object)GetPlayerStateStringById(id, key);
            else if (type == typeof(Vector3))
            {
                string json = GetPlayerStateStringById(id, key);
                if (json != null)
                {
                    return (T)(object)JsonUtility.FromJson<Vector3>(json);
                }
                else
                {
                    return default;
                }
            }
            else if (type == typeof(Color))
            {
                string json = GetPlayerStateStringById(id, key);
                if (json != null)
                {
                    return (T)(object)JsonUtility.FromJson<Color>(json);
                }
                else
                {
                    return default;
                }
            }
            else if (type == typeof(Vector2))
            {
                string json = GetPlayerStateStringById(id, key);
                if (json != null)
                {
                    return (T)(object)JsonUtility.FromJson<Vector2>(json);
                }
                else
                {
                    return default;
                }
            }
            else if (type == typeof(Quaternion))
            {
                string json = GetPlayerStateStringById(id, key);
                if (json != null)
                {
                    return (T)(object)JsonUtility.FromJson<Quaternion>(json);
                }
                else
                {
                    return default;
                }
            }
            else throw new NotSupportedException($"Type {typeof(T)} is not supported by GetState");
        }

        private static bool GetPlayerStateBoolById(string id, string key)
        {
            var stateValue = GetPlayerStateIntById(id, key);
            return stateValue == 1 ? true :
                stateValue == 0 ? false :
                throw new InvalidOperationException($"GetStateBool: {key} is not a bool");
        }


        [DllImport("__Internal")]
        private static extern void KickInternal(string playerID, Action onKickCallBack = null);

        [DllImport("__Internal")]
        private static extern void WaitForPlayerStateInternal(string playerID, string stateKey,
            Action onStateSetCallback = null);

        [DllImport("__Internal")]
        private static extern void SetPlayerStateByPlayerId(string playerID, string key, int value,
            bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetPlayerStateFloatByPlayerId(string playerID, string key, string value,
            bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetPlayerStateByPlayerId(string playerID, string key, bool value,
            bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetPlayerStateDictionary(string playerID, string key, string jsonValues,
            bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetPlayerStateStringById(string playerID, string key, string value,
            bool reliable = false);

        [DllImport("__Internal")]
        private static extern int GetPlayerStateIntById(string playerID, string key);

        [DllImport("__Internal")]
        private static extern float GetPlayerStateFloatById(string playerID, string key);

        [DllImport("__Internal")]
        private static extern string GetPlayerStateStringById(string playerID, string key);


        [DllImport("__Internal")]
        private static extern string GetPlayerStateDictionary(string playerID, string key);

        [DllImport("__Internal")]
        private static extern string GetProfileByPlayerId(string playerID);
    }
}