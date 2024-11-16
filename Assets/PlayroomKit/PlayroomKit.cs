using UnityEngine;
using System.Collections.Generic;
using System;
using UBB;


namespace Playroom
{
    public partial class PlayroomKit
    {
        private readonly IPlayroomBase _playroomService;
        private readonly IRPC _rpc;

        private static PlayroomKit _instance;
        public static bool IsPlayRoomInitialized;
        private static readonly Dictionary<string, Player> Players = new();

        public enum MockModeSelector
        {
            Local,
            Browser
        }

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.Local;

        // Constructor
        public PlayroomKit()
        {
#if !UNITY_EDITOR
                _playroomService = new PlayroomBuildService(new PlayroomKitInterop());
                _rpc = new RPC(this);

#elif UNITY_EDITOR

            if (CurrentMockMode == MockModeSelector.Local)
            {
                _playroomService = new LocalMockPlayroomService();
                _rpc = new RPCLocal();
            }
            else if (CurrentMockMode == MockModeSelector.Browser)
            {
                _playroomService = new BrowserMockService();
                _rpc = new BrowserMockRPC();
            }
#endif
        }

        public PlayroomKit(IPlayroomBase playroomService, IRPC rpc)
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
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return false;
            }

            return _playroomService.IsHost();
        }

        public void OnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }

            _playroomService.OnPlayerJoin(onPlayerJoinCallback);
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
                    if (CurrentMockMode == MockModeSelector.Local)
                    {
                        player = new Player(playerId, new Player.LocalPlayerService(playerId));
                    }
                    else if (CurrentMockMode == MockModeSelector.Browser)
                    {
#if UNITY_EDITOR
                        player = new Player(playerId,
                            new BrowserMockPlayerService(UnityBrowserBridge.Instance, playerId));
#endif
                    }
                }
                else
                {
                    player = new Player(playerId, new Player.PlayerService(playerId));
                }

                Players.Add(playerId, player);
                return player;
            }
        }

        public static Player GetPlayerById(string playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }

#if UNITY_WEBGL
            player = new Player(playerId, new Player.PlayerService(playerId));

            Players.Add(playerId, player);
            return player;
#elif UNITY_EDITOR
            if (CurrentMockMode == MockModeSelector.Local)
            {
                player = new Player(playerId, new Player.LocalPlayerService(playerId));
            }
            else if (CurrentMockMode == MockModeSelector.Browser)
            {
                player = new Player(playerId,
                    new BrowserMockPlayerService(UnityBrowserBridge.Instance, playerId));
            }

            Players.Add(playerId, player);
            return player;
#endif
        }

        public void SetState<T>(string key, T value, bool reliable = false)
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }

            _playroomService.SetState(key, value, reliable);
        }

        public T GetState<T>(string key)
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return default;
            }

            return _playroomService.GetState<T>(key);
        }

        public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
            string onResponseReturn = null)
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }

            _rpc.RpcRegister(name, rpcRegisterCallback, onResponseReturn);
        }

        public void RpcCall(string name, object data, Action callbackOnResponse = null)
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("PlayroomKit is not loaded!. Please make sure to call InsertCoin first.");
                return;
            }

            _rpc.RpcCall(name, data, callbackOnResponse);
        }

        public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
        {
            if (!IsPlayRoomInitialized)
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


        public void OnDisconnect(Action callback)
        {
            _playroomService.OnDisconnect(callback);
        }

        public bool IsStreamScreen()
        {
            if (!IsPlayRoomInitialized)
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
        public void CreateJoyStick(JoystickOptions options)
        {
            _playroomService.CreateJoyStick(options);
        }

        public Dpad DpadJoystick()
        {
            return _playroomService.DpadJoystick();
        }

        public Player MyPlayer()
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return null;
            }

            return _playroomService.MyPlayer();
        }

        public Player Me()
        {
            if (!IsPlayRoomInitialized)
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