using NUnit.Framework;
using Playroom;
using UnityEngine;
// For mocking the IPlayerService interface

namespace Playroom.Tests.Editor
{
    public class PlayerLocalTests
    {
        private PlayroomKit _playroomKit;
        private PlayroomKit.Player _player;
        private PlayroomKit.Player.IPlayerBase _mockPlayerService;

        private string testId = "test_player_id";

        [SetUp]
        public void SetUp()
        {
            PlayroomKit.CurrentMockMode = PlayroomKit.MockModeSelector.Local;
            var _playroomKitService = new LocalMockPlayroomService();
            _playroomKit = new PlayroomKit(_playroomKitService, new PlayroomKit.RPCLocal());
            _playroomKit.InsertCoin(new InitOptions()
            {
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() { { "score", 0 }, },
            }, () => { });
            // Mock the IPlayerService
            _mockPlayerService = new PlayroomKit.Player.LocalPlayerService(testId);

            // Create a new Player object with the mock service
            _player = new PlayroomKit.Player(testId, _mockPlayerService);
        }
        [TearDown]
        public void TearDown()
        {
            // Clean up resources if necessary
            _playroomKit = null;
            _mockPlayerService = null;

            // Reset static states
            PlayroomKit.GetPlayers().Clear();
            // Reset CallbackManager and RPC callbacks
            CallbackManager.ClearAllCallbacks();
            PlayroomKit.RPC.ClearAllCallbacksAndEvents();
        }

        [Test]
        public void WaitForState_RegisterCallback()
        {
            _player.WaitForState("winner", (data) =>
            {
                Debug.Log("winner data: " + data);
                Assert.IsTrue(bool.Parse(data), "Callback should be invoked");
            });
        }

        [Test]
        public void WaitForState_ShouldBeInvokedWhenSetIsSet()
        {
            _player.SetState("winner", true);
        }
    }
}
