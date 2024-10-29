using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using AOT;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Serialization;

namespace Playroom
{
    // Player class
    public partial class PlayroomKit
    {
        public partial class Player
        {
            
            //DI
            public string id;
            IPlayerBase _playerService;

            private static int totalObjects = 0;
            public Player(string playerID, IPlayerBase playerService)
            {
                this.id = playerID;
                this._playerService = playerService;
                totalObjects++;
            }
            
            public void SetState(string key, int value, bool reliable = false)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }
                _playerService.SetState(id, key, value, reliable);
            }

            public void SetState(string key, float value, bool reliable = false)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }
                _playerService.SetState(id, key, value, reliable);
            }
            

            public void SetState(string key, bool value, bool reliable = false)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }
                _playerService.SetState(id, key, value, reliable);
            }

            public void SetState(string key, string value, bool reliable = false)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }
                _playerService.SetState(id, key, value, reliable);
            }

            // Overload for complex objects, which will be serialized to JSON
            public void SetState(string key, object value, bool reliable = false)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }
                _playerService.SetState(id, key, value, reliable);
            }

            
            public T GetState<T>(string key)
            {
                Type type = typeof(T);
                var value = _playerService.GetState<T>(id, key);
                return value;
            }
            
            public Profile GetProfile()
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return default;
                }
                return _playerService.GetProfile(id);
            }
            
            public Action OnQuit(Action<string> callback)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                    return null;
                }

                return _playerService.OnQuit(callback);
            }
            
            public void Kick(Action OnKickCallBack = null)
            {
                if (!isPlayRoomInitialized)
                { 
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return;
                }
                
                _playerService.Kick(id, OnKickCallBack);
            }

            
            //DI END
            
            
            
            [Serializable]
            public class Profile
            {
                [NonSerialized] public UnityEngine.Color color;

                [FormerlySerializedAs("jsonColor")] public PlayerProfileColor playerProfileColor;
                public string name;
                public string photo;

                [Serializable]
                public class PlayerProfileColor
                {
                    public int r;
                    public int g;
                    public int b;
                    public string hexString;
                    public int hex;
                }
            }
            

            
            private void OnQuitDefaultCallback()
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("Playroom not initialized yet! Please call InsertCoin.");
                }

                Players.Remove(id);
            }

            
            
            
            public void WaitForState(string StateKey, Action onStateSetCallback = null)
            {
                if (IsRunningInBrowser())
                {
                    WaitForPlayerStateInternal(id, StateKey, onStateSetCallback);
                }
            }
            

            [DllImport("__Internal")]
            private static extern void KickInternal(string playerID, Action onKickCallBack = null);

            [DllImport("__Internal")]
            private static extern void WaitForPlayerStateInternal(string playerID, string stateKey,
                Action onStateSetCallback = null);
            
            [DllImport("__Internal")]
            private static extern string GetProfileByPlayerId(string playerID);
        }
    }
}