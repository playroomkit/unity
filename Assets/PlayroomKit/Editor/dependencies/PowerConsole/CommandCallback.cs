using System.Collections.Generic;

namespace CI.PowerConsole
{
    public class CommandCallback
    {
        /// <summary>
        /// The command that was entered
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Arguments that were entered after the command
        /// </summary>
        public Dictionary<string, string> Args { get; set; }
    }
}