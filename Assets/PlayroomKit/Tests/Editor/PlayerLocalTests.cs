using System;
using System.Globalization;
using NUnit.Framework;
using NSubstitute; // For mocking the IPlayerService interface
using Playroom;
using UnityEngine;

public class PlayerLocalTests
{
    private PlayroomKit _playroomKit;
    private PlayroomKit.Player _player;
    private PlayroomKit.Player.IPlayerBase _mockPlayerService;
    private PlayroomKit.IInterop _interop;
    
    private string testId = "test_player_id";
    private string testKey = "test_key";

    [SetUp]
    public void SetUp()
    {
        var _playroomKitService = new PlayroomKit.LocalMockPlayroomService();
        _playroomKit = new PlayroomKit(_playroomKitService, new PlayroomKit.RPCLocal());
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() { {"score", 0}, },
        }, () =>
        {
            
        });
        // Mock the IPlayerService
        _mockPlayerService = new PlayroomKit.Player.LocalPlayerService(testId);

        // Create a new Player object with the mock service
        _player = new PlayroomKit.Player(testId, _mockPlayerService);
    }

    [Test]
    public void WaitForState_ShouldInvokeCallback_WhenStateIsSet()
    {
        bool callbackInvoked = false;
        _player.WaitForState("winner",() =>
        {
            callbackInvoked = true;
        });
        
        _player.SetState("winner", true);
        
        Assert.IsTrue(callbackInvoked, "Callback should be invoked");
    }
}
