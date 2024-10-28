using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Playroom;
using NSubstitute;

public class PlayroomKitLocalTests
{
    private PlayroomKit _playroomKit;
    private PlayroomKit.IPlayroomBase _mockPlayroomService;
    private PlayroomKit.IInterop _interop;
    private PlayroomKit.IRPC _rpc;
    
    [SetUp]
    public void SetUp()
    {
        _interop = Substitute.For<PlayroomKit.IInterop>();
        // Initialize the mock PlayroomService
        _mockPlayroomService = new PlayroomKit.LocalMockPlayroomService();
        _rpc = new PlayroomKit.RPC(_playroomKit, _interop);
        // Since PlayroomKit uses a private field for the service, we'll need to simulate it or test through the public API
        _playroomKit = new PlayroomKit(_mockPlayroomService, _rpc);
    }
    
    [Test]
    public void InsertCoin_ShouldBeInvoked()
    {
        var mockPlayroomService = Substitute.For<PlayroomKit.IPlayroomBase>();
        var playroomKit = new PlayroomKit(mockPlayroomService, _rpc);
        
        playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() { {"score", 0}, },
        }, () =>
        {
            
        });
        
        mockPlayroomService.Received(1).InsertCoin(Arg.Any<InitOptions>(),Arg.Any<Action>());
    }

    [Test]
    public void OnPlayerJoin_PlayerShouldBeAdded()
    {
        bool playerJoined = false;
        PlayroomKit.Player testPlayer = null;
        var playerId = "mockplayerID123";
        var playroomKit = new PlayroomKit(new PlayroomKit.LocalMockPlayroomService(), _rpc);
        
        playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() { {"score", 0}, },
        }, () =>
        {
            
        });
        
        playroomKit.OnPlayerJoin(player =>
        {
            playerJoined = true;
            Debug.Log(player.id);
        });
        
        Assert.IsTrue(playerJoined, "Callback should be invoked when player joins.");
    }
    
    [Test]
    public void GetRoomCode_ShouldReturnRoomCode()
    {
        var roomCode = _playroomKit.GetRoomCode();
        Assert.AreEqual(roomCode, "mock123");
    }
    
    [Test]
    public void StartMatchmaking_ShouldInvoke_StartMatchmakingLocal()
    {
        var mockPlayroomService = Substitute.For<PlayroomKit.IPlayroomBase>();
        
        // Since PlayroomKit uses a private field for the service, we'll need to simulate it or test through the public API
        var playroomKit = new PlayroomKit(mockPlayroomService, _rpc);
        playroomKit.StartMatchmaking();
        
        mockPlayroomService.Received(1).StartMatchmaking(Arg.Any<Action>());
    }
    
}
