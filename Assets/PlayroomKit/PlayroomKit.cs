using UnityEngine;
using System.Collections.Generic;
using System;




namespace Playroom
{
    public partial class PlayroomKit
    {
        private IPlayroomBase _playroomService;
        private static PlayroomKit _instance;
        private IRPC _rpc;
        public static bool isPlayRoomInitialized;
        private static readonly Dictionary<string, Player> Players = new();
        
        static Action startMatchmakingCallback = null;

        public PlayroomKit()
        {
            if (IsRunningInBrowser())
            {
                _playroomService = new PlayroomService(new PlayroomKitInterop());
                _rpc = new RPC(this);
            }
            else
            {
                _playroomService = new LocalMockPlayroomService();
                _rpc = new RPCLocal();
            }
        }
        
        public  PlayroomKit(IPlayroomBase playroomService, IRPC rpc)
        {
            _playroomService = playroomService;
            _rpc = rpc;
        }
        
        public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
            Action onDisconnectCallback = null)
        {
            _playroomService.InsertCoin(options, onLaunchCallBack, onDisconnectCallback);
        }
        
        public bool IsHost()
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return false;
            }
            return _playroomService.IsHost();
        }
        
        public Action OnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return null;
            }

            return _playroomService.OnPlayerJoin(onPlayerJoinCallback);
        }
        
        public Player GetPlayer(string playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }
            else
            {
                if (!IsRunningInBrowser())
                {
                    player = new Player(playerId, new Player.LocalPlayerService(playerId));
                }
                else
                {
                    player = new Player(playerId, new Player.PlayerService(playerId));
                }

                Players.Add(playerId, player);
                return player;
            }
        }
        
        private static Player GetPlayerById(string playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }
            else
            {
                if (!IsRunningInBrowser())
                {
                    player = new Player(playerId, new Player.LocalPlayerService(playerId));
                }
                else
                {
                    player = new Player(playerId, new Player.PlayerService(playerId));
                }

                Players.Add(playerId, player);
                return player;
            }
        }
        
        public void SetState<T>(string key, T value, bool reliable = false)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            _playroomService.SetState(key, value, reliable);
        }
        
        public T GetState<T>(string key)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return default;
            }
            return _playroomService.GetState<T>(key);
        }
        
        public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
            string onResponseReturn = null)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            
            _rpc.RpcRegister(name, rpcRegisterCallback, onResponseReturn);
        }
        
        public void RpcCall(string name, object data, Action callbackOnResponse = null)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            
            _rpc.RpcCall(name, data, callbackOnResponse);
        }
        
        public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }
            
            _rpc.RpcCall(name, data, mode, callbackOnResponse);
        }
        
        public void StartMatchmaking(Action callback = null)
        {
            _playroomService.StartMatchmaking(callback);
        }
        
        // it checks if the game is running in the browser or in the editor
        public static bool IsRunningInBrowser()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
                return true;
#else
            return false;
#endif
        }
        
        public static Dictionary<string, Player> GetPlayers()
        {
            return Players;
        }
        
        public string GetRoomCode()
        {
            return _playroomService.GetRoomCode();
        }
        
        
        public  void OnDisconnect(Action callback)
        {
            _playroomService.OnDisconnect(callback);
        }
        
        public bool IsStreamScreen()
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return false;
            }
            
            return _playroomService.IsStreamScreen();
        }
        
        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            _playroomService.WaitForState(stateKey, onStateSetCallback);
        }
        
        public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null)
        {
            _playroomService.WaitForPlayerState(playerID, stateKey, onStateSetCallback);
        }
        
        public void ResetStates(string[] keysToExclude = null, Action OnStatesReset = null)
        {
            _playroomService.ResetStates(keysToExclude, OnStatesReset);
        }
        
        public void ResetPlayersStates(string[] keysToExclude = null, Action OnStatesReset = null)
        {
            _playroomService.ResetPlayersStates(keysToExclude, OnStatesReset);
        }
        
        // Joystick
        public  void CreateJoyStick(JoystickOptions options)
        {
            _playroomService.CreateJoyStick(options);
        }

        public Dpad DpadJoystick()
        {
            return _playroomService.DpadJoystick();
        }
        
        public Player MyPlayer()
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return null;
            }
            return _playroomService.MyPlayer();
        }

        public Player Me()
        {
            if (!isPlayRoomInitialized)
            {
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return null;
            }
            
            return _playroomService.Me();
        }
        
        private void UnsubscribeOnQuit()
        {
            _playroomService.UnsubscribeOnQuit();
        }
        
        // DI END
        
    }
}