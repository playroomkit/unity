using UnityEngine;

namespace CI.PowerConsole
{
    public class ConsoleColours
    {
        /// <summary>
        /// The text colour of trace logs
        /// </summary>
        public Color TraceColour { get; set; } = Color.white;

        /// <summary>
        /// The text colour of debug logs
        /// </summary>
        public Color DebugColor { get; set; } = Color.white;

        /// <summary>
        /// The text colour of information logs
        /// </summary>
        public Color InformationColor { get; set; } = Color.white;

        /// <summary>
        /// The text colour of warning logs
        /// </summary>
        public Color WarningColor { get; set; } = Color.yellow;

        /// <summary>
        /// The text colour of error logs
        /// </summary>
        public Color ErrorColor { get; set; } = Color.red;

        /// <summary>
        /// The text colour of critical logs
        /// </summary>
        public Color CriticalColor { get; set; } = Color.magenta;

        /// <summary>
        /// The text colour of user entered commands
        /// </summary>
        public Color NoneColour { get; set; } = Color.white;
    }
}