using System;

namespace Playroom
{
    /// <summary>
    /// Handles which mode is being used
    /// </summary>
    public partial class PlayroomKit
    {
        public enum MockModeSelector
        {
            Local,
            BrowserBridge
        }

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.Local;

        private static void ExecuteMockModeAction(Action localAction, Action browserAction)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    localAction();
                    break;
#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:
                    browserAction();
                    break;
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static T ExecuteMockModeFunc<T>(Func<T> localFunc, Func<T> browserFunc)
        {
            return CurrentMockMode switch
            {
                MockModeSelector.Local => localFunc(),
#if UNITY_EDITOR
                MockModeSelector.BrowserBridge => browserFunc(),
#endif
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private static void MockInsertCoin(InitOptions options, Action onLaunchCallBack)
        {
            ExecuteMockModeAction(
                () => MockInsertCoinLocal(options, onLaunchCallBack),
                () => MockInsertCoinBrowser(options, onLaunchCallBack));
        }

        private static void MockOnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            ExecuteMockModeAction(
                () => MockOnPlayerJoinLocal(onPlayerJoinCallback),
                () => MockOnPlayerJoinBrowser(onPlayerJoinCallback));
        }

        private static void MockSetState(string key, object value, bool reliable = false)
        {
            ExecuteMockModeAction(
                () => MockSetStateLocal(key, value),
                () => MockSetStateBrowser(key, value, reliable));
        }

        private static void MockSetState(string playerID, string key, object value, bool reliable = false)
        {
            ExecuteMockModeAction(
                () => MockPlayerSetStateLocal(key, value),
                () => MockPlayerSetStateBrowser(playerID, key, value, reliable));
        }

        private static T MockGetState<T>(string key)
        {
            return ExecuteMockModeFunc(
                () => MockGetStateLocal<T>(key),
                () => MockGetStateBrowser<T>(key));
        }

        private static T MockGetState<T>(string playerID, string key)
        {
            return ExecuteMockModeFunc(
                () => MockPlayerGetStateLocal<T>(key),
                () => MockPlayerGetStateBrowser<T>(playerID, key));
        }

        private static string MockGetRoomCode()
        {
            return ExecuteMockModeFunc(
                MockGetRoomCodeLocal,
                MockGetRoomCodeBrowser);
        }

        private static Player MockMyPlayer()
        {
            return ExecuteMockModeFunc(
                MockMyPlayerLocal,
                MockMyPlayerBrowser);
        }

        private static Player MockMe() => MockMyPlayer();

        private static bool MockIsHost()
        {
            return ExecuteMockModeFunc(
                MockIsHostLocal,
                MockIsHostBrowser);
        }

        private static bool MockIsStreamScreen()
        {
            return ExecuteMockModeFunc(
                MockIsStreamScreenLocal,
                MockIsStreamScreenBrowser);
        }

        private static Player.Profile MockGetProfile(string id = null)
        {
            return ExecuteMockModeFunc(
                MockGetProfileLocal,
                () => MockGetProfileBrowser(id)
            );
        }

        private static void MockStartMatchmaking()
        {
            ExecuteMockModeAction(
                MockStartMatchmakingLocal,
                MockStartMatchmakingBrowser);
        }

        private static void MockKick(string id = null, Action onKickCallback = null)
        {
            ExecuteMockModeAction(
                () => MockKickLocal(onKickCallback),
                () => MockKickBrowser(id));
        }

        private static void MockResetStates(string[] keysToExclude = null, Action onKickCallback = null)
        {
            ExecuteMockModeAction(
                () => MockResetStatesLocal(keysToExclude, onKickCallback),
                () => MockResetStatesBrowser(keysToExclude, onKickCallback));
        }

        private static void MockResetPlayersStates(string[] keysToExclude = null, Action onKickCallback = null,
            string serializedKeysToExclude = null)
        {
            ExecuteMockModeAction(
                () => MockResetPlayersStatesLocal(keysToExclude, onKickCallback),
                () => ResetPlayersStatesBrowser(serializedKeysToExclude, onKickCallback));
        }

        private static void MockOnDisconnect(Action callback)
        {
            ExecuteMockModeAction(
                () => MockOnDisconnectLocal(callback),
                () => MockOnDisconnectBrowser(callback));
        }

        private static void MockWaitForState(string state, Action<string> callback)
        {
            ExecuteMockModeAction(
                () => MockWaitForStateLocal(state, callback),
                () => MockWaitForStateBrowser(state, callback));
        }

        private static void MockRpcRegister(string name, Action<string, string> callback, string responseReturn)
        {
            ExecuteMockModeAction(
                () => MockRpcRegisterLocal(name, callback),
                () => MockRpcRegisterBrowser(name, callback));
        }

        private static void MockRpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
        {
            ExecuteMockModeAction(
                () => MockRpcCallLocal(name, data, mode, callbackOnResponse),
                () => MockRpcCallBrowser(name, data, mode, callbackOnResponse));
        }
    }
}