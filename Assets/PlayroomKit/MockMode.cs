using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        private const string PlayerId = "mockPlayer";
        private static bool mockIsStreamMode;

        /* 
        This is private, instead of public, to prevent tampering in Mock Mode.
        Reason: In Mock Mode, only a single player can be tested. 
        Ref: https://docs.joinplayroom.com/usage/unity#mock-mode
        */
        private static Dictionary<string, object> MockDictionary = new();

        public static void MockSetState(string key, object value)
        {
            if (MockDictionary.ContainsKey(key))
                MockDictionary[key] = value;
            else
                MockDictionary.Add(key, value);
        }

        public static T MockGetState<T>(string key)
        {
            if (MockDictionary.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            else
            {
                Debug.LogWarning($"No {key} in States or value is not of type {typeof(T)}");
                return default;
            }
        }
    }

}