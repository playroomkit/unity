using System;


namespace Playroom
{
    /// <summary>
    /// Handles which mode mode is being used
    /// </summary>
    public partial class PlayroomKit
    {
#if UNITY_EDITOR


        public enum MockModeSelector
        {
            MockModeSimulated,
            BrowserBridgeMode
        }

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.MockModeSimulated;
        // public static string MockManagerObjectName { get; set; }

        private static void MockInsertCoin(InitOptions options, Action onLaunchCallBack)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.MockModeSimulated:

                    MockInsertCoinSimulated(options, onLaunchCallBack);
                    break;

                case MockModeSelector.BrowserBridgeMode:

                    MockInsertCoinBrowser(options, onLaunchCallBack);
                    break;
            }
        }


        private static void MockOnPlayerJoin(Action<Player> onPlayerJoinCallback)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.MockModeSimulated:
                    MockOnPlayerJoinSimulated(onPlayerJoinCallback);
                    break;

                case MockModeSelector.BrowserBridgeMode:
                    MockOnPlayerJoinBrowser(onPlayerJoinCallback);
                    break;
            }
        }

        private static void MockSetState(string key, object value, bool reliable = false)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.MockModeSimulated:
                    MockSetStateSimulated(key, value);
                    break;

                case MockModeSelector.BrowserBridgeMode:
                    MockSetStateBrowser(key, value, reliable);
                    break;
            }
        }

        private static void MockSetState(string playerID, string key, object value, bool reliable = false)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.MockModeSimulated:
                    MockSetStateSimulated(key, value);
                    break;

                case MockModeSelector.BrowserBridgeMode:
                    MockPlayerSetStateBrowser(playerID, key, value, reliable);
                    break;
            }
        }

        private static T MockGetState<T>(string key)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.MockModeSimulated:
                    return MockGetStateSimulated<T>(key);

                case MockModeSelector.BrowserBridgeMode:
                    return MockGetStateBrowser<T>(key);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


#endif
    }
}