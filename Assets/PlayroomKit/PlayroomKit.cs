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
        internal static readonly Dictionary<string, Player> Players = new();

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

            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    _playroomService = new LocalMockPlayroomService();
                    _rpc = new RPCLocal();
                    break;

                case MockModeSelector.Browser:
                    _playroomService = new BrowserMockService();
                    _rpc = new BrowserMockRPC();
                    break;
                default:
                    _playroomService = new LocalMockPlayroomService();
                    _rpc = new RPCLocal();
                    break;
            }
#endif
        }

        // for tests
        public PlayroomKit(IPlayroomBase playroomService, IRPC rpc)
        {
            _playroomService = playroomService;
            _rpc = rpc;
        }

        private static void CheckPlayRoomInitialized()
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError("Playroom not initialized yet! Please call InsertCoin.");
                return;
            }
        }

        public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
            Action onDisconnectCallback = null)
        {
            _playroomService.InsertCoin(options, onLaunchCallBack, onDisconnectCallback);
        }

        public void OnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            CheckPlayRoomInitialized();
            _playroomService.OnPlayerJoin(onPlayerJoinCallback);
        }

        public bool IsHost()
        {
            CheckPlayRoomInitialized();

            return _playroomService.IsHost();
        }

        private void TransferHost(string playerId)
        {
            CheckPlayRoomInitialized();

            _playroomService.TransferHost(playerId);
        }

        public Player GetPlayer(string playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }
            else
            {
#if UNITY_EDITOR
                switch (CurrentMockMode)
                {
                    case MockModeSelector.Local:
                        player = new Player(playerId, new Player.LocalPlayerService(playerId));
                        break;
                    case MockModeSelector.Browser:
                        player = new Player(playerId,
                            new BrowserMockPlayerService(UnityBrowserBridge.Instance, playerId));
                        break;
                }
#else
                player = new Player(playerId, new Player.PlayerService(playerId));
#endif
                Players.Add(playerId, player);
                return player;
            }
        }

        public static Player GetPlayerById(string playerId)
        {
            if (!IsPlayRoomInitialized)
            {
                Debug.LogError(IsRunningInBrowser()
                    ? "PlayroomKit is not loaded!. Please make sure to call InsertCoin first."
                    : "[Mock Mode] Playroom not initialized yet! Please call InsertCoin.");
                return default;
            }

            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }
            else
            {
#if UNITY_EDITOR
                if (CurrentMockMode == MockModeSelector.Local)
                {
                    player = new Player(playerId, new Player.LocalPlayerService(playerId));
                }
                else if (CurrentMockMode == MockModeSelector.Browser)
                {
                    player = new Player(playerId,
                        new BrowserMockPlayerService(UnityBrowserBridge.Instance, playerId));
                }
#else
                player = new Player(playerId, new Player.PlayerService(playerId));
#endif
                Players.Add(playerId, player);
                return player;
            }
        }

        public void SetState<T>(string key, T value, bool reliable = false)
        {
            CheckPlayRoomInitialized();

            _playroomService.SetState(key, value, reliable);
        }

        public T GetState<T>(string key)
        {
            CheckPlayRoomInitialized();

            return _playroomService.GetState<T>(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rpcRegisterCallback">A callback which takes 2 string params, 1 </param>
        /// <param name="onResponseReturn"></param>
        public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
            string onResponseReturn = null)
        {
            CheckPlayRoomInitialized();

            _rpc.RpcRegister(name, rpcRegisterCallback, onResponseReturn);
        }

        public void RpcCall(string name, object data, Action callbackOnResponse = null)
        {
            CheckPlayRoomInitialized();

            _rpc.RpcCall(name, data, callbackOnResponse);
        }

        public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
        {
            CheckPlayRoomInitialized();

            _rpc.RpcCall(name, data, mode, callbackOnResponse);
        }

        public void StartMatchmaking(Action callback = null)
        {
            CheckPlayRoomInitialized();
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
            CheckPlayRoomInitialized();
            return _playroomService.GetRoomCode();
        }

        public void OnDisconnect(Action callback)
        {
            _playroomService.OnDisconnect(callback);
        }

        public bool IsStreamScreen()
        {
            CheckPlayRoomInitialized();

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
            CheckPlayRoomInitialized();

            return _playroomService.MyPlayer();
        }

        public Player Me()
        {
            CheckPlayRoomInitialized();
            return _playroomService.Me();
        }

        private void UnsubscribeOnQuit()
        {
            _playroomService.UnsubscribeOnQuit();
        }

        #region Persistence

        public void SetPersistentData(string key, object value)
        {
            CheckPlayRoomInitialized();

            _playroomService.SetPersistentData(key, value);
        }

        public void InsertPersistentData(string key, object value)
        {
            CheckPlayRoomInitialized();
            _playroomService.InsertPersistentData(key, value);
        }

        public void GetPersistentData(string key, Action<string> onGetPersistentDataCallback)
        {
            CheckPlayRoomInitialized();
            _playroomService.GetPersistentData(key, onGetPersistentDataCallback);
        }

        #endregion

        #region Turn Based

        public string GetChallengeId()
        {
            CheckPlayRoomInitialized();
            return _playroomService.GetChallengeId();
        }

        public void SaveMyTurnData(object data)
        {
            CheckPlayRoomInitialized();
            _playroomService.SaveMyTurnData(data);
        }

        public void GetMyTurnData(Action<string> callback)
        {
            CheckPlayRoomInitialized();
            _playroomService.GetMyTurnData(callback);
        }

        public void GetAllTurns(Action<string> callback)
        {
            CheckPlayRoomInitialized();
            _playroomService.GetAllTurns(callback);
        }

        public void ClearTurns(Action callback = null)
        {
            CheckPlayRoomInitialized();
            _playroomService.ClearTurns(callback);
        }

        #endregion
    }
}