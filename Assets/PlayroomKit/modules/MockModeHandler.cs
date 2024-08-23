using System;
using UnityEngine;

namespace Playroom
{
    /// <summary>
    /// Handles which mode mode is being used
    /// </summary>
    public partial class PlayroomKit
    {
        public enum MockModeSelector
        {
            MockModeSimulated,
            BrowserBridgeMode
        }

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.BrowserBridgeMode;


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
    }
}