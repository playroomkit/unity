using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using System;
using SimpleJSON;

namespace Playroom
{
    public class PlayroomKit
    {
        [DllImport("__Internal")]
        public static extern void InsertCoin(Action callback);

        [DllImport("__Internal")]
        private static extern void OnPlayerJoinInternal(Action<string> callback);

        private static Action<Player> onPlayerJoinCallback = null;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void WrapperCallback(string id)
        {
            Player player = new Player(id);

            onPlayerJoinCallback?.Invoke(player);
        }

        public static void OnPlayerJoin(Action<Player> playerCallback)
        {
            onPlayerJoinCallback = playerCallback;
            OnPlayerJoinInternal(WrapperCallback);
        }

        [DllImport("__Internal")]
        public static extern bool IsHost();

        [DllImport("__Internal")]
        private static extern void SetStateString(string key, string value);

        [DllImport("__Internal")]
        public static extern void SetState(string key, int value);

        [DllImport("__Internal")]
        public static extern void SetState(string key, float value);

        [DllImport("__Internal")]
        public static extern void SetState(string key, bool value);

        public static void SetState(string key, string value)
        {
            SetStateString(key, value);
        }

        [DllImport("__Internal")]
        private static extern void SetStateDictionary(string key, string jsonValues);


        public static void SetState(string key, Dictionary<string, int> values)
        {
            SetStateHelper(key, values);
        }

        public static void SetState(string key, Dictionary<string, float> values)
        {
            SetStateHelper(key, values);
        }

        public static void SetState(string key, Dictionary<string, bool> values)
        {
            SetStateHelper(key, values);
        }

        public static void SetState(string key, Dictionary<string, string> values)
        {
            SetStateHelper(key, values);
        }


        // GETTERS
        [DllImport("__Internal")]
        public static extern string GetStateString(string key);

        [DllImport("__Internal")]
        public static extern int GetStateInt(string key);

        [DllImport("__Internal")]
        public static extern float GetStateFloat(string key);

        public static bool GetStateBool(string key)
        {
            if (GetStateInt(key) == 1)
            {
                return true;
            }
            else if (GetStateInt(key) == 0)
            {
                return false;
            }
            else
            {
                Debug.LogError("GetStateBool: " + key + " is not a bool");
                return false;
            }

        }

        [DllImport("__Internal")]
        private static extern string GetStateDictionary(string key);

        public static Dictionary<string, float> GetStateFloatDict(string key)
        {
            string jsonString = GetStateDictionary(key);
            Debug.Log("jsonString: " + jsonString);
            return ParseJsonToDictionary<float>(jsonString);
        }

        // helper functions:
        private static void SetStateHelper<T>(string key, Dictionary<string, T> values)
        {
            JSONObject jsonObject = new JSONObject();

            // Add key-value pairs to the JSON object
            foreach (var kvp in values)
            {
                // Convert the value to double before adding to JSONNode
                double value = Convert.ToDouble(kvp.Value);
                jsonObject.Add(kvp.Key, value);
            }

            // Serialize the JSON object to a string
            string jsonString = jsonObject.ToString();

            // Output the JSON string
            Debug.Log("Serialized JSON: " + jsonString);
            SetStateDictionary(key, jsonString);
        }

        private static Dictionary<string, T> ParseJsonToDictionary<T>(string jsonString)
        {
            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            JSONNode jsonNode = JSON.Parse(jsonString);

            foreach (KeyValuePair<string, JSONNode> kvp in jsonNode.AsObject)
            {
                T value = default; // Initialize the value to default value of T

                // Parse the JSONNode value to the desired type (T)
                if (typeof(T) == typeof(float))
                {
                    value = (T)(object)kvp.Value.AsFloat;
                }
                else if (typeof(T) == typeof(int))
                {
                    value = (T)(object)kvp.Value.AsInt;
                }
                else if (typeof(T) == typeof(bool))
                {
                    value = (T)(object)kvp.Value.AsBool;
                }
                else
                {
                    Debug.LogError("Unsupported type: " + typeof(T).FullName);
                }

                dictionary.Add(kvp.Key, value);
            }

            return dictionary;
        }

        // it checks if the game is running in the browser or in the editor
        public static bool IsRunningInBrowser()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        return true;
#else
            return false;
#endif
        }


        // Player class
        public class Player
        {
            public string id;

            public Player(string id)
            {
                this.id = id;
            }

            public void SetState(string key, int value)
            {
                Debug.Log("player id in setstate unity: " + id);
                SetPlayerStateByPlayerId(id, key, value);
            }

            public void SetState(string key, float value)
            {
                SetPlayerStateByPlayerId(id, key, value);
            }

            public void SetState(string key, bool value)
            {
                SetPlayerStateByPlayerId(id, key, value);
            }

            public void SetState(string key, string value)
            {
                SetPlayerStateStringById(id, key, value);
            }

            public int GetStateInt(string key)
            {
                return GetPlayerStateIntById(id, key);
            }

            public float GetStateFloat(string key)
            {
                return GetPlayerStateFloatById(id, key);
            }

            public string GetStateString(string key)
            {
                return GetPlayerStateStringById(id, key);
            }

            public bool GetStateBool(string key)
            {
                if (GetPlayerStateIntById(id, key) == 1)
                {
                    return true;
                }
                else if (GetPlayerStateIntById(id, key) == 0)
                {
                    return false;
                }
                else
                {
                    Debug.LogError("GetPlayerStateByPlayerId: " + key + " is not a bool");
                    return false;
                }

            }

            public void SetState(string key, Dictionary<string, int> values)
            {
                SetStateHelper(id, key, values);
            }

            public void SetState(string key, Dictionary<string, float> values)
            {
                SetStateHelper(id, key, values);
            }

            public void SetState(string key, Dictionary<string, bool> values)
            {
                SetStateHelper(id, key, values);
            }

            public void SetState(string key, Dictionary<string, string> values)
            {
                SetStateHelper(id, key, values);
            }

            public Dictionary<string, float> GetStateFloat(string id, string key)
            {
                string jsonString = GetPlayerStateDictionary(id, key);
                Debug.Log("jsonString: " + jsonString);
                return ParseJsonToDictionary<float>(jsonString);
            }


            [DllImport("__Internal")]
            private static extern void SetPlayerStateByPlayerId(string playerID, string key, int value);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateByPlayerId(string playerID, string key, float value);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateByPlayerId(string playerID, string key, bool value);

            [DllImport("__Internal")]
            private static extern void SetPlayerStateStringById(string playerID, string key, string value);

            [DllImport("__Internal")]
            public static extern string GetProfileByPlayerId(string playerID);  // returning hexColor

            [DllImport("__Internal")]
            private static extern int GetPlayerStateIntById(string playerID, string key);

            [DllImport("__Internal")]
            private static extern float GetPlayerStateFloatById(string playerID, string key);

            [DllImport("__Internal")]
            private static extern string GetPlayerStateStringById(string playerID, string key);

            // Helpers
            [DllImport("__Internal")]
            private static extern void SetPlayerStateDictionary(string playerID, string key, string jsonValues);

            [DllImport("__Internal")]
            private static extern string GetPlayerStateDictionary(string playerID, string key);

            private void SetStateHelper<T>(string id, string key, Dictionary<string, T> values)
            {
                JSONObject jsonObject = new JSONObject();

                // Add key-value pairs to the JSON object
                foreach (var kvp in values)
                {
                    // Convert the value to double before adding to JSONNode
                    double value = Convert.ToDouble(kvp.Value);
                    jsonObject.Add(kvp.Key, value);
                }

                // Serialize the JSON object to a string
                string jsonString = jsonObject.ToString();

                // Output the JSON string
                Debug.Log("Serialized JSON: " + jsonString);
                SetPlayerStateDictionary(id, key, jsonString);
            }


        }

    }

}