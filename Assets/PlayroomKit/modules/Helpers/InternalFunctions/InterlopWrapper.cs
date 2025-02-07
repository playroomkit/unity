using System;
using System.Runtime.InteropServices;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class PlayroomKitInterop : IInterop
        {
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

            public void TransferHostWrapper(string playerId)
            {
                TransferHostInternal(playerId);
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

            public void RpcRegisterWrapper(string name, Action<string> rpcRegisterCallback,
                string onResponseReturn = null)
            {
                RpcRegisterInternal(name, rpcRegisterCallback, onResponseReturn);
            }

            public void RpcCallWrapper(string name, string data, RpcMode mode, Action callbackOnResponse)
            {
                RpcCallInternal(name, data, mode, callbackOnResponse);
            }


            //Internal Functions
            [DllImport("__Internal")]
            private static extern void RpcRegisterInternal(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null);

            [DllImport("__Internal")]
            private static extern void RpcRegisterInternal(string name, Action<string> rpcRegisterCallback,
                string onResponseReturn = null);

            [DllImport("__Internal")]
            private extern static void RpcCallInternal(string name, string data, RpcMode mode,
                Action callbackOnResponse);


            //Player 
            // Wrapper for KickInternal
            public void KickPlayerWrapper(string playerID, Action onKickCallback = null)
            {
                KickInternal(playerID, onKickCallback);
            }

            // Wrapper for WaitForPlayerStateInternal
            public void WaitForPlayerStateWrapper(string playerID, string stateKey,
                Action<string> onStateSetCallback = null)
            {
                WaitForPlayerStateInternal(playerID, stateKey, onStateSetCallback);
            }

            // Wrapper for SetPlayerStateByPlayerId (int version)
            public void SetPlayerStateIntWrapper(string playerID, string key, int value, bool reliable = false)
            {
                SetPlayerStateByPlayerId(playerID, key, value, reliable);
            }

            // Wrapper for SetPlayerStateFloatByPlayerId
            public void SetPlayerStateFloatWrapper(string playerID, string key, string value, bool reliable = false)
            {
                SetPlayerStateFloatByPlayerId(playerID, key, value, reliable);
            }

            // Wrapper for SetPlayerStateByPlayerId (bool version)
            public void SetPlayerStateBoolWrapper(string playerID, string key, bool value, bool reliable = false)
            {
                SetPlayerStateByPlayerId(playerID, key, value, reliable);
            }

            // Wrapper for SetPlayerStateDictionary
            public void SetPlayerStateDictionaryWrapper(string playerID, string key, string jsonValues,
                bool reliable = false)
            {
                SetPlayerStateDictionary(playerID, key, jsonValues, reliable);
            }

            // Wrapper for SetPlayerStateStringById
            public void SetPlayerStateStringWrapper(string playerID, string key, string value, bool reliable = false)
            {
                SetPlayerStateStringById(playerID, key, value, reliable);
            }

            // Wrapper for GetPlayerStateIntById
            public int GetPlayerStateIntWrapper(string playerID, string key)
            {
                return GetPlayerStateIntById(playerID, key);
            }

            // Wrapper for GetPlayerStateFloatById
            public float GetPlayerStateFloatWrapper(string playerID, string key)
            {
                return GetPlayerStateFloatById(playerID, key);
            }

            // Wrapper for GetPlayerStateStringById
            public string GetPlayerStateStringWrapper(string playerID, string key)
            {
                return GetPlayerStateStringById(playerID, key);
            }

            // Wrapper for GetPlayerStateDictionary
            public string GetPlayerStateDictionaryWrapper(string playerID, string key)
            {
                return GetPlayerStateDictionary(playerID, key);
            }

            // Wrapper for GetProfileByPlayerId
            public string GetProfileWrapper(string playerID)
            {
                return GetProfileByPlayerId(playerID);
            }

            public void SetPersistentDataWrapper(string key, string value)
            {
                SetPersistentDataInternal(key, value);
            }

            public void InsertPersistentDataWrapper(string key, string value)
            {
                InsertPersistentDataInternal(key, value);
            }

            public void GetPersistentDataWrapper(string key, Action<string, string> onGetPersistentDataCallback)
            {
                GetPersistentDataInternal(key, onGetPersistentDataCallback);
            }


            #region Turn based

            public string GetChallengeIdWrapper()
            {
                return GetChallengeIdInternal();
            }

            public void SaveMyTurnDataWrapper(string data)
            {
                SaveMyTurnDataInternal(data);
            }

            public void GetAllTurnsWrapper(Action<string> callback)
            {
                GetAllTurnsInternal(callback);
            }

            public void GetMyTurnDataWrapper(Action<string> callback)
            {
                GetMyTurnDataInternal(callback);
            }

            public void ClearTurnsWrapper(Action callback = null)
            {
                ClearTurnsInternal(callback);
            }

            #endregion
        }
    }
}