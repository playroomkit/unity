using System;
using System.Runtime.InteropServices;


namespace Playroom
{
    public partial class PlayroomKit
    {
        [DllImport("__Internal")]
        private static extern void InsertCoinInternal(string options,
            Action<string> onLaunchCallback,
            Action<string> onQuitInternalCallback,
            Action<string> onDisconnectCallback,
            Action<string> onError,
            string onLaunchCallBackKey,
            string onDisconnectCallBackKey);

        [DllImport("__Internal")]
        private static extern string OnPlayerJoinInternal(Action<string> callback);

        [DllImport("__Internal")]
        private static extern void UnsubscribeOnPlayerJoinInternal(string id);

        [DllImport("__Internal")]
        private static extern bool IsHostInternal();

        [DllImport("__Internal")]
        private static extern bool TransferHostInternal(string playerId);

        [DllImport("__Internal")]
        private static extern bool IsStreamScreenInternal();

        [DllImport("__Internal")]
        private static extern string MyPlayerInternal();

        [DllImport("__Internal")]
        private static extern string GetRoomCodeInternal();

        [DllImport("__Internal")]
        private static extern void OnDisconnectInternal(Action<string> callback);

        [DllImport("__Internal")]
        private static extern void SetStateString(string key, string value, bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetStateInternal(string key, int value, bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetStateInternal(string key, bool value, bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetStateFloatInternal(string key, string floatAsString, bool reliable = false);

        [DllImport("__Internal")]
        private static extern void SetStateDictionary(string key, string jsonValues, bool reliable = false);

        [DllImport("__Internal")]
        private static extern string GetStateStringInternal(string key);

        [DllImport("__Internal")]
        private static extern int GetStateIntInternal(string key);

        [DllImport("__Internal")]
        private static extern float GetStateFloatInternal(string key);

        [DllImport("__Internal")]
        private static extern string GetStateDictionaryInternal(string key);

        [DllImport("__Internal")]
        private static extern void WaitForStateInternal(string stateKey, Action<string, string> onStateSetCallback);


        [DllImport("__Internal")]
        private static extern void ResetStatesInternal(string keysToExclude = null, Action OnStatesReset = null);

        [DllImport("__Internal")]
        private static extern void ResetPlayersStatesInternal(string keysToExclude, Action OnPlayersStatesReset = null);

        [DllImport("__Internal")]
        private static extern void UnsubscribeOnQuitInternal();

        [DllImport("__Internal")]
        private static extern void CreateJoystickInternal(string joyStickOptionsJson);

        [DllImport("__Internal")]
        private static extern string DpadJoystickInternal();

        [DllImport("__Internal")]
        private static extern void StartMatchmakingInternal(Action callback);

        [DllImport("__Internal")]
        private static extern void KickInternal(string playerID, Action onKickCallback = null);

        [DllImport("__Internal")]
        private static extern void WaitForPlayerStateInternal(string playerID, string stateKey,
            Action<string> onStateSetCallback = null);

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

        #region Persistence

        [DllImport("__Internal")]
        private static extern void SetPersistentDataInternal(string key, string value);

        [DllImport("__Internal")]
        private static extern void InsertPersistentDataInternal(string key, string value);

        [DllImport("__Internal")]
        private static extern string GetPersistentDataInternal(string key,
            Action<string, string> OnGetPersistentDataCallback);

        #endregion

        #region TurnBased

        [DllImport("__Internal")]
        private static extern string GetChallengeIdInternal();

        [DllImport("__Internal")]
        private static extern void SaveMyTurnDataInternal(string data);

        [DllImport("__Internal")]
        private static extern string GetAllTurnsInternal(Action<string> callback);

        [DllImport("__Internal")]
        private static extern string GetMyTurnDataInternal(Action<string> callback);

        [DllImport("__Internal")]
        private static extern void ClearTurnsInternal(Action callback = null);

        #endregion
    }
}