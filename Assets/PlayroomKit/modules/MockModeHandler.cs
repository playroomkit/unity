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
                () => MockInsertCoinSimulated(options, onLaunchCallBack),
                () => MockInsertCoinBrowser(options, onLaunchCallBack));
        }

        private static void MockOnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            ExecuteMockModeAction(
                () => MockOnPlayerJoinSimulated(onPlayerJoinCallback),
                () => MockOnPlayerJoinBrowser(onPlayerJoinCallback));
        }

        private static void MockSetState(string key, object value, bool reliable = false)
        {
            ExecuteMockModeAction(
                () => MockSetStateSimulated(key, value),
                () => MockSetStateBrowser(key, value, reliable));
        }

        private static void MockSetState(string playerID, string key, object value, bool reliable = false)
        {
            ExecuteMockModeAction(
                () => MockSetStateSimulated(key, value),
                () => MockPlayerSetStateBrowser(playerID, key, value, reliable));
        }

        private static T MockGetState<T>(string key)
        {
            return ExecuteMockModeFunc(
                () => MockGetStateSimulated<T>(key),
                () => MockGetStateBrowser<T>(key));
        }

        private static T MockGetState<T>(string playerID, string key)
        {
            return ExecuteMockModeFunc(
                () => MockGetStateSimulated<T>(key),
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
    }
}