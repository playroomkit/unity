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
        public interface IPlayerInteraction
        {
            void InvokeOnQuitWrapperCallback();
        }

        public class Player : IPlayerInteraction
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

            public void SetState(string key, object value, bool reliable = false)
            {
                _playerService.SetState(id, key, value, reliable);
            }
            
            public T GetState<T>(string key)
            {
                Type type = typeof(T);
                var value = _playerService.GetState<T>(id, key);
                return value;
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
            

            private List<Action<string>> OnQuitCallbacks = new();


            private void OnQuitDefaultCallback()
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("Playroom not initialized yet! Please call InsertCoin.");
                }

                Players.Remove(id);
            }

            [MonoPInvokeCallback(typeof(Action))]
            private void OnQuitWrapperCallback()
            {
                if (OnQuitCallbacks != null)
                    foreach (var callback in OnQuitCallbacks)
                        callback?.Invoke(id);
            }

            void IPlayerInteraction.InvokeOnQuitWrapperCallback()
            {
                OnQuitWrapperCallback();
            }

            public Action OnQuit(Action<string> callback)
            {
                if (!isPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                    return null;
                }
                else
                {
                    OnQuitCallbacks.Add(callback);

                    void Unsubscribe()
                    {
                        OnQuitCallbacks.Remove(callback);
                    }

                    return Unsubscribe;
                }
            }

            
            public void WaitForState(string StateKey, Action onStateSetCallback = null)
            {
                if (IsRunningInBrowser())
                {
                    WaitForPlayerStateInternal(id, StateKey, onStateSetCallback);
                }
            }

            public void Kick(Action OnKickCallBack = null)
            {
                if (IsRunningInBrowser())
                {
                    onKickCallBack = OnKickCallBack;
                    KickInternal(id, InvokeKickCallBack);
                }
                else
                {
                    if (!isPlayRoomInitialized)
                    {
                        Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    }
                    else
                    {
                        MockKick(id, OnKickCallBack);
                    }
                }
            }


            public Profile GetProfile()
            {
                if (IsRunningInBrowser())
                {
                    var jsonString = GetProfileByPlayerId(id);
                    var profileData = ParseProfile(jsonString);
                    return profileData;
                }

                if (isPlayRoomInitialized) return MockGetProfile(id);
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return default;
            }


            private static Action onKickCallBack = null;

            [MonoPInvokeCallback(typeof(Action))]
            private static void InvokeKickCallBack()
            {
                onKickCallBack?.Invoke();
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