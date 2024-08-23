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

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.MockModeSimulated;
    }
}