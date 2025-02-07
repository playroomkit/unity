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

            void TransferHostWrapper(string playerId);

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

            void WaitForPlayerStateWrapper(string playerID, string stateKey, Action<string> onStateSetCallback);

            void ResetStatesWrapper(string keysToExclude = null, Action OnStatesReset = null);

            void ResetPlayersStatesWrapper(string keysToExclude, Action OnPlayersStatesReset = null);

            void UnsubscribeOnQuitWrapper();

            void CreateJoystickWrapper(string joyStickOptionsJson);

            string DpadJoystickWrapper();

            void StartMatchmakingWrapper(Action callback);

            void RpcRegisterWrapper(string name, Action<string> rpcRegisterCallback,
                string onResponseReturn = null);

            void RpcCallWrapper(string name, string data, RpcMode mode,
                Action callbackOnResponse);


            //Player Functions
            void KickPlayerWrapper(string playerID, Action onKickCallback = null);

            void SetPlayerStateIntWrapper(string playerID, string key, int value, bool reliable = false);

            void SetPlayerStateFloatWrapper(string playerID, string key, string value, bool reliable = false);

            void SetPlayerStateBoolWrapper(string playerID, string key, bool value, bool reliable = false);

            void SetPlayerStateDictionaryWrapper(string playerID, string key, string jsonValues, bool reliable = false);

            void SetPlayerStateStringWrapper(string playerID, string key, string value, bool reliable = false);

            int GetPlayerStateIntWrapper(string playerID, string key);

            float GetPlayerStateFloatWrapper(string playerID, string key);

            string GetPlayerStateStringWrapper(string playerID, string key);

            string GetPlayerStateDictionaryWrapper(string playerID, string key);

            string GetProfileWrapper(string playerID);

            //
            void SetPersistentDataWrapper(string key, string value);
            void InsertPersistentDataWrapper(string key, string value);

            void GetPersistentDataWrapper(string key, Action<string, string> onGetPersistentDataCallback);

            #region Turnbased

            string GetChallengeIdWrapper();

            void SaveMyTurnDataWrapper(string data);

            void GetAllTurnsWrapper(Action<string> callback);

            void GetMyTurnDataWrapper(Action<string> callback);

            void ClearTurnsWrapper(Action callback = null);

            #endregion
        }
    }
}