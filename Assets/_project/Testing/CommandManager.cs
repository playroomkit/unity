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
                InformationColor = new Color(0,0.5f,1),
                TraceColour = new Color(0.5f,0.5f,0.5f),
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
            new() { Command = "InsertCoin", Description = "Starts multiplayer", Callback = InsertCoinCommand },
            new() { Command = "SetState", Description = "Sets State", Callback = SetStateCommand },
            new() { Command = "GetState", Description = "Gets State", Callback = GetStateCommand },
            new() { Command = "GetRoomCode", Description = "Gets Room Code", Callback = GetRoomCodeCommand },
            new() { Command = "StartMatchMaking", Description = "Starts matchmaking", Callback = StartMatchMakingCommand },
            new() { Command = "IsHost", Description = "Returns true if current player is host", Callback = IsHostCommand },
            new() { Command = "IsStreamScreen", Description = "Returns true if current screen is streamer screen", Callback = IsStreamScreenCommand },
            new() { Command = "MyPlayer", Description = "Returns current player's object", Callback = MyPlayerCommand },
            new() { Command = "Me", Description = "Returns current player's object", Callback = MeCommand },
            new() { Command = "OnPlayerJoin", Description = "Displays ID of player who joined", Callback = OnPlayerJoinCommand },
            new() { Command = "WaitForState", Description = "Waits for state to finish", Callback = WaitForStateCommand },
            new() { Command = "OnDisconnect", Description = "Callback on player disconnect", Callback = OnDisconnectCommand },
            new() { Command = "ResetState", Description = "Reset state to default", Callback = ResetStateCommand }
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
        string[] keyToExclude = cmd.Args["-keyToExclude"].ToString().Split(',');
        _prk.ResetStates(keyToExclude);
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
        PowerConsole.Log(LogLevel.Information, !string.IsNullOrEmpty(value) ? $"State value for key '{key}': {value}" : $"No value found for key '{key}'.");
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
        PowerConsole.Log(LogLevel.Information, $"My Player's Name: {_prk.MyPlayer().GetProfile().name}");
    }

    private void MeCommand(CommandCallback cmd)
    {
        LogCommand("Me");
        MyPlayerCommand(cmd);
        PowerConsole.Log(LogLevel.Information, $"My Player's Name: {_prk.MyPlayer().GetProfile().name}");
    }

    private void InsertCoinCommand(CommandCallback cmd)
    {
        LogCommand("InsertCoin");
        
        bool defaultSkipLobby = false;
        int defaultMaxPlayer = 2;
        bool defaultDiscordMode = false;
        bool defaultStreamMode = false;
        bool dAllowGamePad = false; 
        string defaultUrl = "https://example.com"; 
        int defaultReconnectGracePoint = 5; 
        bool defaultMatchMaking = false;
        string[] defaultAvatars = new string[]{ "avatar1.png", "avatar2.png" };
        string romcd = "123";
        string gameID = "111";
        
        bool lobby = cmd.Args.ContainsKey("-skipLobby") ? Convert.ToBoolean(cmd.Args["-skipLobby"]):defaultSkipLobby;
        int? maxNumbersOfPlayer = cmd.Args.ContainsKey("-maxPlayers") ? Convert.ToInt32(cmd.Args["-maxPlayers"]):defaultMaxPlayer;
        bool discord = cmd.Args.ContainsKey("-discord") ? Convert.ToBoolean(cmd.Args["-discord"]):defaultDiscordMode;
        bool stmMode = cmd.Args.ContainsKey("-streamMode") ? Convert.ToBoolean(cmd.Args["-streamMode"]): defaultStreamMode;
        bool agpad =  cmd.Args.ContainsKey("-allowGamePad") ? Convert.ToBoolean(cmd.Args["-allowGamePad"]): dAllowGamePad;
        string burl =  cmd.Args.ContainsKey("-baseUrl") ? Convert.ToString(cmd.Args["-baseUrl"]):defaultUrl;
        int rgp =  cmd.Args.ContainsKey("-reconnectGracePeriod") ? Convert.ToInt32(cmd.Args["-reconnectGracePeriod"]):defaultReconnectGracePoint;
        bool mtchMkng =  cmd.Args.ContainsKey("-matchMaking") ? Convert.ToBoolean(cmd.Args["-matchMaking"]): defaultMatchMaking;
        string[] _avatars = cmd.Args.ContainsKey("-avatars") ? cmd.Args["-avatars"].ToString().Split(',') : defaultAvatars;
        string roomCode = cmd.Args.ContainsKey("-roomCode") ? cmd.Args["-roomCode"] : romcd;
        string gmeID = cmd.Args.ContainsKey("-gameId") ? cmd.Args["-gameId"] : gameID;
        
        InitOptions io = new InitOptions
        {
            skipLobby = lobby,
            roomCode = roomCode,
            maxPlayersPerRoom = maxNumbersOfPlayer,
            gameId = gmeID, 
            discord = discord,
            streamMode = stmMode,
            allowGamepads = agpad,
            baseUrl = burl,
            reconnectGracePeriod = rgp,
            avatars = _avatars,
            matchmaking = mtchMkng,
        };

        // actually call playroom's api
        _prk.InsertCoin(
            io,
            OnLaunchCallBack
        );
    }
    private void OnLaunchCallBack()
    {
        PowerConsole.Log(LogLevel.Trace, "OnLaunch Invoked");
    }
}

