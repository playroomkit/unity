using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Playroom;
using NSubstitute;

namespace Playroom.Tests.Editor
{
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
            _mockPlayroomService = new LocalMockPlayroomService();
            _rpc = new PlayroomKit.RPC(_playroomKit, _interop);
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
                defaultPlayerStates = new() { { "score", 0 }, },
            }, () => { });

            mockPlayroomService.Received(1).InsertCoin(Arg.Any<InitOptions>(), Arg.Any<Action>());
        }

        [Test]
        public void OnPlayerJoin_PlayerShouldBeAdded()
        {
            bool playerJoined = false;
            var playroomKit = new PlayroomKit(new LocalMockPlayroomService(), _rpc);

            playroomKit.InsertCoin(new InitOptions()
            {
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() { { "score", 0 }, },
            }, () => { });

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

        [Test]
        public void IsStreamScreen_ShouldReturnFalse_WhenCalled()
        {
            _playroomKit.InsertCoin(new InitOptions()
            {
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() { { "score", 0 }, },
            }, () => { });

            var isStream = _playroomKit.IsStreamScreen();
            Debug.Log(isStream);
            Assert.IsFalse(isStream, "IsStreamScreen should be false.");
        }

        [Test]
        public void ResetStates_ShouldInvokeCallback()
        {
            var keysToExclude = new[] { "pos" };
            bool callbackInvoked = false;

            // Act
            _playroomKit.ResetStates(keysToExclude, () => callbackInvoked = true);

            Assert.IsTrue(callbackInvoked, "Callback should be invoked");
        }

        [Test]
        public void ResetPlayersStates_InvokeCallback()
        {
            // Arrange
            var keysToExclude = new[] { "pos" };
            bool callbackInvoked = false;

            // Act
            _playroomKit.ResetPlayersStates(keysToExclude, () => callbackInvoked = true);

            //Assert
            Assert.IsTrue(callbackInvoked, "Callback should be invoked");
        }

        [Test]
        public void MyPlayer_ReturnLocalPlayer()
        {
            var expectedPlayer = _playroomKit.GetPlayer("mockplayerID123");
            var player = _playroomKit.MyPlayer();
            Assert.AreEqual(expectedPlayer, player);
        }


        [Test]
        public void WaitForState_ShouldInvokeCallback_WhenStateIsSet()
        {
            bool callbackInvoked = false;
            _playroomKit.WaitForState("winner", key => { callbackInvoked = true; });

            _playroomKit.SetState("winner", true);

            Assert.IsTrue(callbackInvoked, "Callback should be invoked");
        }
    }
}