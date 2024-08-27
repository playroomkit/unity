using System;


namespace Playroom
{
    /// <summary>
    /// Handles which mode mode is being used
    /// </summary>
    public partial class PlayroomKit
    {
        public enum MockModeSelector
        {
            Local,
            BrowserBridge
        }

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.Local;
        // public static string MockManagerObjectName { get; set; }

        private static void MockInsertCoin(InitOptions options, Action onLaunchCallBack)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:

                    MockInsertCoinSimulated(options, onLaunchCallBack);
                    break;
#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:

                    MockInsertCoinBrowser(options, onLaunchCallBack);
                    break;
#endif
            }
        }


        private static void MockOnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    MockOnPlayerJoinSimulated(onPlayerJoinCallback);
                    break;
#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:
                    MockOnPlayerJoinBrowser(onPlayerJoinCallback);
                    break;
#endif
            }
        }

        private static void MockSetState(string key, object value, bool reliable = false)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    MockSetStateSimulated(key, value);
                    break;
#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:
                    MockSetStateBrowser(key, value, reliable);
                    break;
#endif
            }
        }

        private static void MockSetState(string playerID, string key, object value, bool reliable = false)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    MockSetStateSimulated(key, value);
                    break;

#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:
                    MockPlayerSetStateBrowser(playerID, key, value, reliable);
#endif
                    break;
            }
        }

        private static T MockGetState<T>(string key)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    return MockGetStateSimulated<T>(key);

#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:
                    return MockGetStateBrowser<T>(key);
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static T MockGetState<T>(string playerID, string key)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    return MockGetStateSimulated<T>(key);

#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:
                    return MockPlayerGetStateBrowser<T>(playerID, key);
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}