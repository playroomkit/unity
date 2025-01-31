using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Playroom
{
    [Serializable]
    public class InitOptions
    {
        public bool streamMode = false;
        public bool allowGamepads = false;
        public string baseUrl = "";
        public string[] avatars = null;
        public string roomCode = "";
        public bool skipLobby = false;
        public int reconnectGracePeriod = 0;
        public int? maxPlayersPerRoom;
        [CanBeNull]
        public string gameId;
        public bool discord = false;
        
        public bool persistentMode = false;

        public Dictionary<string, object> defaultStates = null;
        public Dictionary<string, object> defaultPlayerStates = null;

        private object matchmakingField;

        // Property to handle matchmaking as either boolean or MatchMakingOptions
        public object matchmaking
        {
            get => matchmakingField;
            set
            {
                if (value is bool || value is MatchMakingOptions)
                {
                    matchmakingField = value;
                }
                else
                {
                    throw new ArgumentException(
                        "matchmaking must be either a boolean or a MatchMakingOptions object.");
                }
            }
        }
    }
}