using System;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class PlayroomKitInterop : IInterop
        {
            // Wrap the static DllImport calls in instance methods

            public void InsertCoinWrapper(string options,
                Action<string> onLaunchCallback,
                Action<string> onQuitCallback,
                Action<string> onDisconnectCallback,
                Action<string> onError,
                string onLaunchCallbackKey,
                string onDisconnectCallbackKey)
            {
                // Call the static DllImport method
                InsertCoinInternal(options, onLaunchCallback, onQuitCallback, onDisconnectCallback, onError,
                    onLaunchCallbackKey, onDisconnectCallbackKey);
            }

            public string OnPlayerJoinWrapper(Action<string> callback)
            {
                return OnPlayerJoinInternal(callback);
            }

            public void UnsubscribeOnPlayerJoinWrapper(string id)
            {
                UnsubscribeOnPlayerJoinInternal(id);
            }

            public bool IsHostWrapper()
            {
                return IsHostInternal();
            }

            public bool IsStreamScreenWrapper()
            {
                return IsStreamScreenInternal();
            }

            public string MyPlayerWrapper()
            {
                return MyPlayerInternal();
            }

            public string GetRoomCodeWrapper()
            {
                return GetRoomCodeInternal();
            }

            public void OnDisconnectWrapper(Action<string> callback)
            {
                OnDisconnectInternal(callback);
            }

            public void SetStateStringWrapper(string key, string value, bool reliable = false)
            {
                SetStateString(key, value, reliable);
            }

            public void SetStateIntWrapper(string key, int value, bool reliable = false)
            {
                SetStateInternal(key, value, reliable);
            }

            public void SetStateBoolWrapper(string key, bool value, bool reliable = false)
            {
                SetStateInternal(key, value, reliable);
            }

            public void SetStateFloatWrapper(string key, string floatAsString, bool reliable = false)
            {
                SetStateFloatInternal(key, floatAsString, reliable);
            }

            public void SetStateDictionaryWrapper(string key, string jsonValues, bool reliable = false)
            {
                SetStateDictionary(key, jsonValues, reliable);
            }

            public string GetStateStringWrapper(string key)
            {
                return GetStateStringInternal(key);
            }

            public int GetStateIntWrapper(string key)
            {
                return GetStateIntInternal(key);
            }

            public float GetStateFloatWrapper(string key)
            {
                return GetStateFloatInternal(key);
            }

            public string GetStateDictionaryWrapper(string key)
            {
                return GetStateDictionaryInternal(key);
            }

            public void WaitForStateWrapper(string stateKey, Action<string, string> onStateSetCallback)
            {
                WaitForStateInternal(stateKey, onStateSetCallback);
            }

            public void WaitForPlayerStateWrapper(string playerID, string stateKey, Action onStateSetCallback)
            {
                WaitForPlayerStateInternal(playerID, stateKey, onStateSetCallback);
            }

            public void ResetStatesWrapper(string keysToExclude = null, Action OnStatesReset = null)
            {
                ResetStatesInternal(keysToExclude, OnStatesReset);
            }

            public void ResetPlayersStatesWrapper(string keysToExclude, Action OnPlayersStatesReset = null)
            {
                ResetPlayersStatesInternal(keysToExclude, OnPlayersStatesReset);
            }

            public void UnsubscribeOnQuitWrapper()
            {
                UnsubscribeOnQuitInternal();
            }

            public void CreateJoystickWrapper(string joyStickOptionsJson)
            {
                CreateJoystickInternal(joyStickOptionsJson);
            }

            public string DpadJoystickWrapper()
            {
                return DpadJoystickInternal();
            }

            public void StartMatchmakingWrapper(Action callback)
            {
                StartMatchmakingInternal(callback);
            }
        }
    }
}