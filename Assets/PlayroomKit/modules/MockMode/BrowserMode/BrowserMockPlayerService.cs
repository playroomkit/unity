using System;
using System.Collections.Generic;
using UBB;
using UnityEngine;

namespace Playroom
{
#if UNITY_EDITOR
    public class BrowserMockPlayerService : PlayroomKit.Player.IPlayerBase
    {
        private readonly UnityBrowserBridge _ubb;
        private readonly string _id;

        public BrowserMockPlayerService(UnityBrowserBridge ubb, string id)
        {
            _ubb = ubb;
            _id = id;
        }

        #region State

        public void SetState(string key, int value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, float value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, bool value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, string value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, object value, bool reliable = false)
        {
            string json;

            // Check if the value is a primitive type and wrap it if necessary
            json = JsonUtility.ToJson(value is int or float or bool or string
                ? new PrimitiveWrapper<object>(value)
                : value);

            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, json,
                reliable.ToString().ToLower());
        }

        public T GetState<T>(string key)
        {
            string stringValue = _ubb.CallJs<string>("GetPlayerStateByPlayerId", null, null, false, _id, key);

            if (string.IsNullOrEmpty(stringValue))
            {
                Debug.LogWarning($"State for key '{key}' is null or empty.");
                return default;
            }

            try
            {
                if (typeof(T) == typeof(int) || typeof(T) == typeof(float) || typeof(T) == typeof(bool) ||
                    typeof(T) == typeof(string))
                {
                    var wrapper = JsonUtility.FromJson<PrimitiveWrapper<T>>(stringValue);
                    return wrapper.value;
                }
                else
                {
                    return JsonUtility.FromJson<T>(stringValue);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON for key '{key}': {e.Message}");
                return default; // Return default if parsing fails.
            }
        }

        #endregion

        public PlayroomKit.Player.Profile GetProfile()
        {
            string json = _ubb.CallJs<string>("GetProfile", null, null, false, _id);

            DebugLogger.Log("Profile Json: " + json);

            var profileData = Helpers.ParseProfile(json);
            return profileData;
        }

        public void Kick(Action onKickCallBack = null)
        {
            _ubb.CallJs("Kick", null, null, true, _id);
        }

        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            string callbackKey = $"WaitForState_{stateKey}";
            GameObject callbackObject = new GameObject(callbackKey);
            Debug.Log(callbackKey);

            MockCallbackInvoker invoker = callbackObject.AddComponent<MockCallbackInvoker>();
            invoker.SetCallback(onStateSetCallback, callbackKey);
            CallBacksHandlerMock.Instance.RegisterCallbackObject(callbackKey, callbackObject, "ExecuteCallback");

            _ubb.CallJs("WaitForPlayerState", null, null, true, _id, stateKey, callbackKey);
        }
        
        private List<Action<string>> OnQuitCallbacks = new();

        public Action OnQuit(Action<string> callback)
        {
            OnQuitCallbacks.Add(callback);

            void Unsubscribe()
            {
                OnQuitCallbacks.Remove(callback);
            }

            return Unsubscribe;
        }
        
        public void OnQuitWrapperCallback(string id)
        {
            if (OnQuitCallbacks != null)
                foreach (var callback in OnQuitCallbacks)
                    callback?.Invoke(id);
        }
        
        internal void InvokePlayerOnQuitCallback(string id)
        {
            OnQuitWrapperCallback(id);
        }

        #region UTILS

        [Serializable]
        private class PrimitiveWrapper<T>
        {
            public T value;
            public PrimitiveWrapper(T value) => this.value = value;
        }

        #endregion
    }
#endif
}