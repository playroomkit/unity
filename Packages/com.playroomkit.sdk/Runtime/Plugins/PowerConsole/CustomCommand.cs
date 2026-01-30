using System;
using System.Collections.Generic;

namespace CI.PowerConsole
{
    public class CustomCommand
    {
        /// <summary>
        /// Optional arguments that can be added after the command beginning with - or --. Shown by the help command
        /// </summary>
        public List<CommandArgument> Args { get; set; }

        /// <summary>
        /// The command
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Optional description of the command. Shown by the help command
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Callback raised when this command is entered
        /// </summary>
        public Action<CommandCallback> Callback { get; set; }
    }
}