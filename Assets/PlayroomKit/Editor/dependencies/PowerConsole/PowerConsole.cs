using System;
using System.Collections.Generic;
using System.Linq;
using CI.PowerConsole.Core;
using UnityEngine;

namespace CI.PowerConsole
{
    public static class PowerConsole
    {
        /// <summary>
        /// Should the console collect logs
        /// </summary>
        public static bool IsEnabled
        {
            get => _controller.IsEnabled;
            set => _controller.IsEnabled = value;
        }

        /// <summary>
        /// Is the console visible to the user
        /// </summary>
        public static bool IsVisible
        {
            get => _controller.IsVisible;
            set => _controller.IsVisible = value;
        }

        /// <summary>
        /// Sets the minimum log level. Logs below this level won't be collected
        /// </summary>
        public static LogLevel LogLevel
        {
            get => _controller.LogLevel;
            set => _controller.LogLevel = value;
        }

        /// <summary>
        /// Sets the title of the console
        /// </summary>
        public static string Title
        {
            get => _controller.Title;
            set => _controller.Title = value;
        }

        /// <summary>
        /// The key or keys that need to be pressed to open or close the console
        /// </summary>
        public static List<KeyCode> OpenCloseHotkeys
        {
            get => _controller.OpenCloseHotkeys;
            set => _controller.OpenCloseHotkeys = value;
        }

        /// <summary>
        /// Raised when the user enters a command
        /// </summary>
        public static event EventHandler<CommandEnteredEventArgs> CommandEntered;

        private static ConsoleController _controller;

        /// <summary>
        /// Initialises the console. Call this once before attempting to interact with the console
        /// </summary>
        public static void Initialise() => Initialise(new ConsoleConfig());

        /// <summary>
        /// Initialises the console with the specified configuration. Call this once before attempting to interact with the console
        /// </summary>
        /// <param name="config">The configuration</param>
        public static void Initialise(ConsoleConfig config)
        {
            if (_controller == null)
            {
                _controller = UnityEngine.Object.FindObjectsOfType<ConsoleController>(true).First();
                _controller.gameObject.SetActive(true);
                _controller.CommandEntered += (s, e) => CommandEntered?.Invoke(s, e);

                _controller.Initialise(config);
            }
        }

        /// <summary>
        /// Logs a message to the console
        /// </summary>
        /// <param name="logLevel">The level to log this message at</param>
        /// <param name="message">The message to log</param>
        public static void Log(LogLevel logLevel, string message)
        {
            if (IsEnabled && logLevel >= LogLevel)
            {
                _controller.Log(logLevel, message, false);
            }
        }

        /// <summary>
        /// Clears all text from the console
        /// </summary>
        public static void Clear()
        {
            if (IsEnabled)
            {
                _controller.ClearDisplay();
            }
        }

        /// <summary>
        /// Registers a command. If the command already exists it will be overwritten
        /// </summary>
        /// <param name="command">The command to add</param>
        public static void RegisterCommand(CustomCommand command) => _controller.RegisterCommand(command);

        /// <summary>
        /// Unregisters the specified command if it exists
        /// </summary>
        /// <param name="command">The command to remove</param>
        public static void UnregisterCommand(string command) => _controller.UnregisterCommand(command); 

        /// <summary>
        /// Determines whether the specified command is registered
        /// </summary>
        /// <param name="command">The command to check</param>
        /// <returns>True if the command is registered, otherwise false</returns>
        public static bool CommandExists(string command) => _controller.CommandExists(command);
    }
}