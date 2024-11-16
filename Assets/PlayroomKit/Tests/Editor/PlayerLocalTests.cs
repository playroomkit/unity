using NUnit.Framework;
using Playroom;
using UnityEngine;
// For mocking the IPlayerService interface

namespace Playroom.Tests.Editor
{
    public class PlayerLocalTests
    {
        private Playroom.PlayroomKit _playroomKit;
        private Playroom.PlayroomKit.Player _player;
        private Playroom.PlayroomKit.Player.IPlayerBase _mockPlayerService;
        private Playroom.PlayroomKit.IInterop _interop;

        private string testId = "test_player_id";

        [SetUp]
        public void SetUp()
        {
            var _playroomKitService = new LocalMockPlayroomService();
            _playroomKit = new Playroom.PlayroomKit(_playroomKitService, new Playroom.PlayroomKit.RPCLocal());
            _playroomKit.InsertCoin(new InitOptions()
            {
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() { { "score", 0 }, },
            }, () => { });
            // Mock the IPlayerService
            _mockPlayerService = new Playroom.PlayroomKit.Player.LocalPlayerService(testId);

            // Create a new Player object with the mock service
            _player = new Playroom.PlayroomKit.Player(testId, _mockPlayerService);
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