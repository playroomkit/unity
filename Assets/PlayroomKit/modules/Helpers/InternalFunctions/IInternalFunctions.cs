using System;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public interface IInterop
        {
            void InsertCoinWrapper(string options,
                Action<string> onLaunchCallback,
                Action<string> onQuitInternalCallback,
                Action<string> onDisconnectCallback,
                Action<string> onError,
                string onLaunchCallBackKey,
                string onDisconnectCallBackKey);

            string OnPlayerJoinWrapper(Action<string> callback);

            void UnsubscribeOnPlayerJoinWrapper(string id);

            bool IsHostWrapper();

            bool IsStreamScreenWrapper();

            string MyPlayerWrapper();

            string GetRoomCodeWrapper();

            void OnDisconnectWrapper(Action<string> callback);

            void SetStateStringWrapper(string key, string value, bool reliable = false);

            void SetStateIntWrapper(string key, int value, bool reliable = false);

            void SetStateBoolWrapper(string key, bool value, bool reliable = false);

            void SetStateFloatWrapper(string key, string floatAsString, bool reliable = false);

            void SetStateDictionaryWrapper(string key, string jsonValues, bool reliable = false);

            string GetStateStringWrapper(string key);

            int GetStateIntWrapper(string key);

            float GetStateFloatWrapper(string key);

            string GetStateDictionaryWrapper(string key);

            void WaitForStateWrapper(string stateKey, Action<string, string> onStateSetCallback);

            void WaitForPlayerStateWrapper(string playerID, string stateKey, Action onStateSetCallback);

            void ResetStatesWrapper(string keysToExclude = null, Action OnStatesReset = null);

            void ResetPlayersStatesWrapper(string keysToExclude, Action OnPlayersStatesReset = null);

            void UnsubscribeOnQuitWrapper();

            void CreateJoystickWrapper(string joyStickOptionsJson);

            string DpadJoystickWrapper();

            void StartMatchmakingWrapper(Action callback);
            
            void RpcRegisterWrapper(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null);
            
            void RpcCallWrapper(string name, string data, RpcMode mode,
                Action callbackOnResponse);
        }
    }
}