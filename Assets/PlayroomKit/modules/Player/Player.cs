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
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }

                _playerService.SetState(key, value, reliable);
            }

            public void SetState(string key, float value, bool reliable = false)
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }

                _playerService.SetState(key, value, reliable);
            }


            public void SetState(string key, bool value, bool reliable = false)
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }

                _playerService.SetState(key, value, reliable);
            }

            public void SetState(string key, string value, bool reliable = false)
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }

                _playerService.SetState(key, value, reliable);
            }

            // Overload for complex objects, which will be serialized to JSON
            public void SetState(string key, object value, bool reliable = false)
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded! Please make sure to call InsertCoin first.");
                    return;
                }

                _playerService.SetState(key, value, reliable);
            }


            public T GetState<T>(string key)
            {
                Type type = typeof(T);
                var value = _playerService.GetState<T>(key);
                return value;
            }

            public Profile GetProfile()
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return default;
                }

                return _playerService.GetProfile();
            }

            public Action OnQuit(Action<string> callback)
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                    return null;
                }

                return _playerService.OnQuit(callback);
            }

            public void Kick(Action OnKickCallBack = null)
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                    return;
                }

                _playerService.Kick(OnKickCallBack);
            }

            public void WaitForState(string StateKey, Action<string> onStateSetCallback = null)
            {
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("Playroom not initialized yet! Please call InsertCoin.");
                }

                _playerService.WaitForState(StateKey, onStateSetCallback);
            }


            public void InvokePlayerOnQuitCallback()
            {
#if UNITY_WEBGL
                if (_playerService is PlayerService playerService)
                {
                    playerService.InvokePlayerOnQuitCallback(id);
                }
#endif

#if UNITY_EDITOR
                if (_playerService is BrowserMockPlayerService playerService2)
                {
                    playerService2.InvokePlayerOnQuitCallback(id);
                }
#endif
            }


            [Serializable]
            public class Profile
            {
                [NonSerialized]
                public UnityEngine.Color color;
                
                public PlayerProfileColor playerProfileColor;
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
                if (!IsPlayRoomInitialized)
                {
                    Debug.LogError("Playroom not initialized yet! Please call InsertCoin.");
                }

                Players.Remove(id);
            }
        }
    }
}