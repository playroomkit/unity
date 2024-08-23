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
                    Debug.Log("Normal");
                    MockInsertCoinSimulated(options, onLaunchCallBack);
                    break;

                case MockModeSelector.BrowserBridgeMode:
                    Debug.Log("BrowserMode");
                    MockInsertCoinBrowser(options, onLaunchCallBack);
                    break;

                default:
                    MockInsertCoinSimulated(options, onLaunchCallBack);
                    break;
            }
        }
    }
}