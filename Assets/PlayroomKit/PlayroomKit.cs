using UnityEngine;
using System.Collections.Generic;
using System;


namespace Playroom
{
    public partial class PlayroomKit
    {
        private IPlayroomService _playroomService;
        private static PlayroomKit _instance;
        private IRPC _rpc;
        public static bool isPlayRoomInitialized;
        private static readonly Dictionary<string, Player> Players = new();
        

        private PlayroomKit()
        {
            if (MockHandler.isMock)
            {
                Debug.Log("MockHandler.isMock" + MockHandler.isMock);
                _playroomService = new LocalMockPlayroomService();
                _rpc = new RPCLocal();
                Debug.Log("Mock playroom service initialized");
            }
            else
            {
                Debug.Log("MockHandler.isMock" + MockHandler.isMock);
                _playroomService = new PlayroomService(new PlayroomKitInterop());
                _rpc = new RPC();
                Debug.Log("Build playroom service initialized");
            }
        }

        public  PlayroomKit(IPlayroomService playroomService)
        {
            _playroomService = playroomService;
        }
        
        // Singleton Instance
        public static PlayroomKit Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayroomKit();
                }
                return _instance;
            }
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

        public static Player GetPlayer(string playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
            {
                return player;
            }
            else
            {
                if (MockHandler.isMock)
                {
                    player = new Player(playerId, new MockPlayerService());
                }
                else
                {
                    player = new Player(playerId, new PlayerService());
                }

                Players.Add(playerId, player);
                return player;
            }
        }

        static Action startMatchmakingCallback = null;

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
        
    }
}