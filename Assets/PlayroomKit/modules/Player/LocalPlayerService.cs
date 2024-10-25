using System.Collections.Generic;
using UnityEngine;

namespace Playroom
{
    public class LocalPlayerService : IPlayerBase
    {

        private static Dictionary<string, object> mockPlayerStatesDictionary = new();

        public void SetState(string id, string key, object value, bool reliable = false)
        {
            Debug.Log($"MockPlayerService setState: {key} => {value}");
            if (mockPlayerStatesDictionary.ContainsKey(key))
                mockPlayerStatesDictionary[key] = value;
            else
                mockPlayerStatesDictionary.Add(key, value);
        }

        public T GetState<T>(string id, string key)
        {
            if (mockPlayerStatesDictionary.TryGetValue(key, out var value) && value is T typedValue)
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