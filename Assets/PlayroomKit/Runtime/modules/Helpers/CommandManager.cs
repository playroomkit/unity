using System;
using System.Collections.Generic;
using CI.PowerConsole;
using OpenQA.Selenium;
using Playroom;
using UnityEngine;
using LogLevel = CI.PowerConsole.LogLevel;

public class CommandManager : MonoBehaviour
{
    private InitOptions _initOptions;
    private PlayroomKit _prk;

    private void Awake()
    {
        PowerConsole.Initialise(new ConsoleConfig
        {
            IsFixed = false,
            Colours =
            {
                InformationColor = new Color(0, 0.5f, 1),
                TraceColour = new Color(0.5f, 0.5f, 0.5f),
                DebugColor = new Color(0.5f, 0.5f, 0.5f),
            }
        });
        _prk = new PlayroomKit();
    }

    private void Start()
    {
        RegisterCommands();
    }

    private void RegisterCommands()
    {
        var commands = new List<CustomCommand>
        {
            new()
            {
                Command = "InsertCoin",
                Description =
                    "Starts multiplayer -skipLobby (true/false) -roomCode (roomCode for Current room(string)) -maxPlayers (Max number of players per room (int) -discord (discord mode (true/false)) -gameId (game id from dev portal(string)) -streamMode(true/false) -allowGamepads (allows gamepads (true/false)) -baseUrl (provides base URL (string) -avatars (Array of string(image.png) -reconnectGracePeriod (int) -matchmaking (matchmaking field)   )   ",
                Callback = InsertCoinCommand
            },
            new()
            {
                Command = "SetState",
                Description =
                    "Sets State  -key(Maps a value to key (string) -value (value assigned to a key (of type T)) -reliable (value assigned to a key (of type T))  ",
                Callback = SetStateCommand
            },
            new()
            {
                Command = "GetState", Description = "Gets State  -key (Gets the value using a key (string))",
                Callback = GetStateCommand
            },
            new() { Command = "GetRoomCode", Description = "Gets Room Code  - ", Callback = GetRoomCodeCommand },
            new()
            {
                Command = "StartMatchMaking", Description = "Starts matchmaking", Callback = StartMatchMakingCommand
            },
            new()
            {
                Command = "IsHost", Description = "Returns true if current player is host", Callback = IsHostCommand
            },
            new()
            {
                Command = "IsStreamScreen", Description = "Returns true if current screen is streamer screen",
                Callback = IsStreamScreenCommand
            },
            new() { Command = "MyPlayer", Description = "Returns current player's object", Callback = MyPlayerCommand },
            new() { Command = "Me", Description = "Returns current player's object", Callback = MeCommand },
            new()
            {
                Command = "OnPlayerJoin", Description = "Displays ID of player who joined",
                Callback = OnPlayerJoinCommand
            },
            new()
            {
                Command = "WaitForState", Description = "Waits for state to finish -statekey(string)",
                Callback = WaitForStateCommand
            },
            new()
            {
                Command = "OnDisconnect", Description = "Callback on player disconnect", Callback = OnDisconnectCommand
            },
            new()
            {
                Command = "ResetState", Description = "Reset state to default -keysToExclude (string)",
                Callback = ResetStateCommand
            },

            new()
            {
                Command = "Player.GetProfile", Description = "Returns player profile",
                Callback = PlayerGetProfileCommand
            },

            new()
            {
                Command = "Player.GetState", Description = "Returns Playrooms Player State  -key(string)",
                Callback = PlayerGetStateCommand
            },

            new()
            {
                Command = "Player.SetState",
                Description = "Sets Playrooms Player State  -key(string) -value(string) -reliable(true/false)",
                Callback = PlayerSetStateCommand
            },

            new()
            {
                Command = "Player.WaitForState ",
                Description = "Waits for state of Playrooms Player -stateKey(string) ",
                Callback = PlayerWaitForStateCommand
            },

            new()
            {
                Command = "Player.Kick", Description = "Kicks PLayer -playerId(string)", Callback = KickPlayerCommand
            },
            new()
            {
                Command = "Player.OnQuit", Description = "Registers a Call back when a player quits the room",
                Callback = PlayerOnQuitCommand
            },
            new()
            {
                Command = "RpcRegister", Description = "Registers a Remote Procedural Call -name(string)",
                Callback = RpcRegisterCommand
            },
            new()
            {
                Command = "RpcCall", Description = "Calls a Remote Procedural Call -name(string) -data(string) ",
                Callback = RpcCallCommand
            },
            new()
            {
                Command = "ResetPlayerState", Description = "Resets MyPlayer states -excludeKeys(string)  ",
                Callback = ResetPlayerStatesCommand
            }
        };

        foreach (var command in commands)
        {
            PowerConsole.RegisterCommand(command);
        }
    }

    private void LogCommand(string commandName)
    {
        PowerConsole.Log(LogLevel.Trace, $"Executing command: {commandName}");
    }

    private void OnPlayerJoinCommand(CommandCallback cmd)
    {
        LogCommand("OnPlayerJoin");
        _prk.OnPlayerJoin(player => PowerConsole.Log(LogLevel.Information, $"Player joined: {player.id}"));
    }

    private void ResetStateCommand(CommandCallback cmd)
    {
        LogCommand("ResetState");
        string[] keysToExclude = cmd.Args.ContainsKey("-keysToExclude")
            ? cmd.Args["-keysToExclude"].Split(',')
            : new string[] { "defaultKey" };
        _prk.ResetStates(keysToExclude);
    }

    private void OnDisconnectCommand(CommandCallback cmd)
    {
        LogCommand("OnDisconnect");
        _prk.OnDisconnect(() => PowerConsole.Log(LogLevel.Information, "Player Disconnected"));
    }

    private void WaitForStateCommand(CommandCallback cmd)
    {
        LogCommand("WaitForState");
        string key = Convert.ToString(cmd.Args["-stateKey"]);
        _prk.WaitForState(key, data => PowerConsole.Log(LogLevel.Information, data.ToString()));
    }

    private void SetStateCommand(CommandCallback cmd)
    {
        LogCommand("SetState");

        string key = Convert.ToString(cmd.Args["-key"]);
        string value = Convert.ToString(cmd.Args["-value"]);
        bool reliable = Convert.ToBoolean(cmd.Args["-reliable"]);

        _prk.SetState(key, value, reliable);
    }

    private void GetStateCommand(CommandCallback cmd)
    {
        LogCommand("GetState");
        string key = Convert.ToString(cmd.Args["-key"]);
        string value = _prk.GetState<string>(key);
        PowerConsole.Log(LogLevel.Information,
            !string.IsNullOrEmpty(value)
                ? $"State value for key '{key}': {value}"
                : $"No value found for key '{key}'.");
    }

    private void GetRoomCodeCommand(CommandCallback cmd)
    {
        LogCommand("GetRoomCode");
        PowerConsole.Log(LogLevel.Information, $"Room Code for current room: {_prk.GetRoomCode()}");
    }

    private void StartMatchMakingCommand(CommandCallback cmd)
    {
        LogCommand("StartMatchMaking");
        _prk.StartMatchmaking();
    }

    private void IsHostCommand(CommandCallback cmd)
    {
        LogCommand("IsHost");
        PowerConsole.Log(LogLevel.Information, $"Is Host: {_prk.IsHost()}");
    }

    private void IsStreamScreenCommand(CommandCallback cmd)
    {
        LogCommand("IsStreamScreen");
        PowerConsole.Log(LogLevel.Information, $"Is Stream Screen: {_prk.IsStreamScreen()}");
    }

    private void MyPlayerCommand(CommandCallback cmd)
    {
        LogCommand("MyPlayer");
        PowerConsole.Log(LogLevel.Information,
            $"Player Id {_prk.MyPlayer().id}, My Player's Name: {_prk.MyPlayer().GetProfile().name}");
    }

    private void MeCommand(CommandCallback cmd)
    {
        LogCommand("Me");
        PowerConsole.Log(LogLevel.Information, $"My Player's Name: {_prk.Me().GetProfile().name}");
    }

    private void PlayerGetProfileCommand(CommandCallback cmd)
    {
        PowerConsole.Log(LogLevel.Information, $"Name is : {_prk.MyPlayer().GetProfile().name}");
        PowerConsole.Log(LogLevel.Information, $"Color is :{_prk.MyPlayer().GetProfile().color}");
    }

    private void RpcRegisterCommand(CommandCallback cmd)
    {
        LogCommand("RpcRegister");
        string name = Convert.ToString(cmd.Args["-name"]);
        _prk.RpcRegister(name, (data, sender) =>
            PowerConsole.Log(LogLevel.Information, $"Received RPC from {sender}: {data}"));
    }

    private void RpcCallCommand(CommandCallback cmd)
    {
        LogCommand("RpcCall");

        string name = Convert.ToString(cmd.Args["-name"]);
        string data = Convert.ToString(cmd.Args["-data"]);

        _prk.RpcCall(name, data, () => PowerConsole.Log(LogLevel.Information, $"RPC Call '{name}' completed."));
    }


    private void PlayerGetStateCommand(CommandCallback cmd)
    {
        LogCommand("Player.GetState");
        string key = Convert.ToString(cmd.Args["-key"]);
        string value = _prk.MyPlayer().GetState<string>(key);
        PowerConsole.Log(LogLevel.Information,
            !string.IsNullOrEmpty(value)
                ? $"State value for key '{key}': {value}"
                : $"No value found for key '{key}'.");
    }

    private void PlayerSetStateCommand(CommandCallback cmd)
    {
        LogCommand("Player.SetState");

        string key = cmd.Args.ContainsKey("-key") ? Convert.ToString(cmd.Args["-key"]) : "defaultKey";
        string value = cmd.Args.ContainsKey("-value") ? Convert.ToString(cmd.Args["-value"]) : "defaultValue";
        bool reliable = cmd.Args.ContainsKey("-reliable") ? Convert.ToBoolean(cmd.Args["-reliable"]) : false;

        _prk.MyPlayer().SetState(key, value, reliable);
    }

    private void PlayerWaitForStateCommand(CommandCallback cmd)
    {
        LogCommand("Player.WaitForState");
        string key = Convert.ToString(cmd.Args["-stateKey"]);
        _prk.MyPlayer().WaitForState(key, data => PowerConsole.Log(LogLevel.Information, data.ToString()));
    }

    private void KickPlayerCommand(CommandCallback cmd)
    {
        LogCommand("Player.Kick");

        string playerId = cmd.Args.ContainsKey("-playerId") ? cmd.Args["-playerId"] : _prk.MyPlayer().id;

        var playerToKick = _prk.GetPlayer(playerId);
        if (playerToKick == null)
        {
            PowerConsole.Log(LogLevel.Warning, $"Player with ID '{playerId}' not found.");
            return;
        }

        playerToKick.Kick();

        PowerConsole.Log(LogLevel.Information, $"{playerToKick.GetProfile().name} has been kicked.");
    }

    private void PlayerOnQuitCommand(CommandCallback cmd)
    {
        LogCommand("Player.OnQuit");

        _prk.MyPlayer().OnQuit(_ =>
        {
            string playerName = _prk.MyPlayer().GetProfile().name;
            PowerConsole.Log(LogLevel.Information, $"{playerName} has quit the match.");
        });
    }

    private void ResetPlayerStatesCommand(CommandCallback cmd)
    {
        LogCommand("ResetPlayerState");

        string[] keysToExclude = cmd.Args.ContainsKey("-excludeKeys")
            ? Convert.ToString(cmd.Args["-excludeKeys"]).Split(',') 
            : new string[] { "defaultKey" };

        _prk.ResetPlayersStates(keysToExclude,
            () => { PowerConsole.Log(LogLevel.Information, "Player states have been successfully reset."); });
    }


    private void InsertCoinCommand(CommandCallback cmd)
    {
        LogCommand("InsertCoin");

        bool skipLobby = cmd.Args.ContainsKey("-skipLobby") && Convert.ToBoolean(cmd.Args["-skipLobby"]);
        bool discord = cmd.Args.ContainsKey("-discord") && Convert.ToBoolean(cmd.Args["-discord"]);
        bool streamMode = cmd.Args.ContainsKey("-streamMode") && Convert.ToBoolean(cmd.Args["-streamMode"]);
        bool allowGamepads = cmd.Args.ContainsKey("-allowGamepads") && Convert.ToBoolean(cmd.Args["-allowGamepads"]);
        bool matchmaking = cmd.Args.ContainsKey("-matchmaking") && Convert.ToBoolean(cmd.Args["-matchmaking"]);
        string baseUrl = cmd.Args.ContainsKey("-baseUrl") ? Convert.ToString(cmd.Args["-baseUrl"]) : "";
        int reconnectGracePeriod = cmd.Args.ContainsKey("-reconnectGracePeriod")
            ? Convert.ToInt32(cmd.Args["-reconnectGracePeriod"])
            : 0;
        int? maxPlayersPerRoom = cmd.Args.ContainsKey("-maxPlayersPerRoom")
            ? Convert.ToInt32(cmd.Args["-maxPlayersPerRoom"])
            : null;
        string[] avatars = cmd.Args.ContainsKey("-avatars") ? cmd.Args["-avatars"].ToString().Split(',') : null;
        string roomCode = cmd.Args.ContainsKey("-roomCode") ? cmd.Args["-roomCode"] : "";
        string gameId = cmd.Args.ContainsKey("-gameId") ? cmd.Args["-gameId"] : "";

        InitOptions initOptions = new InitOptions
        {
            skipLobby = skipLobby,
            roomCode = roomCode,
            maxPlayersPerRoom = maxPlayersPerRoom,
            gameId = gameId,
            discord = discord,
            streamMode = streamMode,
            allowGamepads = allowGamepads,
            baseUrl = baseUrl,
            reconnectGracePeriod = reconnectGracePeriod,
            avatars = avatars,
            matchmaking = matchmaking,
        };

        _prk.InsertCoin(
            initOptions,
            OnLaunchCallBack
        );
    }

    private void OnLaunchCallBack()
    {
        PowerConsole.Log(LogLevel.Trace, "OnLaunch Invoked");
    }
}