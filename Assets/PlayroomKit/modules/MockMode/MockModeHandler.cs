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
        }

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.Local;
    }
}