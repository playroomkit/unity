
namespace CI.PowerConsole
{
    public class ConsoleConfig
    {
        /// <summary>
        /// Set colours for each log level
        /// </summary>
        public ConsoleColours Colours { get; set; }

        /// <summary>
        /// The number of log messages the terminal will keep in memory. The default is 100 - setting this too high could affect performance
        /// </summary>
        public int MaxBufferSize { get; set; }

        /// <summary>
        /// The position of the terminal
        /// </summary>
        public ConsolePosition Position { get; set; }

        /// <summary>
        /// The height of the terminal. Ignored if the Postion is Fullscreen
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The width of the terminal. Null to stretch across the screen, ignored if the Postion is Fullscreen
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Can the terminal be dragged
        /// </summary>
        public bool IsFixed { get; set; }

        public ConsoleConfig()
        {
            Colours = new ConsoleColours();
            MaxBufferSize = 100;
            Position = ConsolePosition.Top;
            Height = 400;
            Width = null;
            IsFixed = false;
        }
    }
}