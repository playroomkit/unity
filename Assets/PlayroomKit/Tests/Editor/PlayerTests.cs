using System;
using System.Globalization;
using NUnit.Framework;
using NSubstitute; // For mocking the IPlayerService interface
using Playroom;
using UnityEngine;

namespace Playroom.Tests.Editor
{
    public class PlayerTests
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
            _playroomKit = new PlayroomKit();
            _playroomKit.InsertCoin(new InitOptions()
            {
                maxPlayersPerRoom = 2,
                defaultPlayerStates = new() { { "score", 0 }, },
            }, () => { });
            _interop = Substitute.For<PlayroomKit.IInterop>();
            // Mock the IPlayerService
            _mockPlayerService = new PlayroomKit.Player.PlayerService(testId, _interop);

            // Create a new Player object with the mock service
            _player = new PlayroomKit.Player(testId, _mockPlayerService);
        }

        [Test]
        public void SetState_Int_CallsIntWrapperWithCorrectParameters()
        {
            // Arrange
            int testValue = 100;
            bool reliable = true;

            // Act
            _player.SetState(testKey, testValue, reliable);

            // Assert
            _interop.Received(1).SetPlayerStateIntWrapper(testId, testKey, testValue, reliable);
        }

        [Test]
        public void SetState_Float_CallsFloatWrapperWithCorrectParameters()
        {
            // Arrange
            float testValue = 3.14f;
            bool reliable = false;

            // Act
            _player.SetState(testKey, testValue, reliable);

            // Assert
            _interop.Received(1)
                .SetPlayerStateFloatWrapper(testId, testKey, testValue.ToString(CultureInfo.InvariantCulture),
                    reliable);
        }

        [Test]
        public void SetState_Bool_CallsBoolWrapperWithCorrectParameters()
        {
            // Arrange
            bool testValue = true;
            bool reliable = true;

            // Act
            _player.SetState(testKey, testValue, reliable);

            // Assert
            _interop.Received(1).SetPlayerStateBoolWrapper(testId, testKey, testValue, reliable);
        }

        [Test]
        public void SetState_String_CallsStringWrapperWithCorrectParameters()
        {
            // Arrange
            string testValue = "TestValue";
            bool reliable = false;

            // Act
            _player.SetState(testKey, testValue, reliable);

            // Assert
            _interop.Received(1).SetPlayerStateStringWrapper(testId, testKey, testValue, reliable);
        }

        [Test]
        public void SetState_Object_CallsObjectWrapperWithSerializedJson()
        {
            // Arrange
            var testObject = new { testProperty = "Test" };
            bool reliable = true;
            string expectedJson = JsonUtility.ToJson(testObject);

            // Act
            _player.SetState(testKey, testObject, reliable);

            // Assert
            _interop.Received(1).SetPlayerStateStringWrapper(testId, testKey, expectedJson, reliable);
        }


        [Test]
        public void GetState_ShouldReturnCorrectValue_FromPlayerService()
        {
            // Arrange
            string key = "health";
            int expectedValue = 100;

            // Mock the return value from GetState
            _mockPlayerService.GetState<int>(key).Returns(expectedValue);

            // Act
            int result = _player.GetState<int>(key);

            // Assert
            Assert.AreEqual(expectedValue, result, "GetState should return the correct value.");
        }

        [Test]
        public void GetState_ShouldWorkWithDifferentTypes()
        {
            // Arrange
            string stringKey = "name";
            string expectedStringValue = "Player1";

            string intKey = "score";
            int expectedIntValue = 200;

            // Mock the return values from GetState
            _mockPlayerService.GetState<string>(stringKey).Returns(expectedStringValue);
            _mockPlayerService.GetState<int>(intKey).Returns(expectedIntValue);

            // Act
            string name = _player.GetState<string>(stringKey);
            int score = _player.GetState<int>(intKey);

            // Assert
            Assert.AreEqual(expectedStringValue, name, "GetState should return the correct string value.");
            Assert.AreEqual(expectedIntValue, score, "GetState should return the correct int value.");
        }

        [Test]
        public void GetProfile_ShouldReturnProfileWithParsedData()
        {
            // Mock JSON string that GetProfileWrapper would return
            string jsonString =
                "{\"name\": \"PlayerName\", \"photo\": \"player_photo_url\", \"color\": { \"r\": 255, \"g\": 128, \"b\": 64, \"hexString\": \"#FF8040\", \"hex\": 16744448 }}";

            // Define the expected Profile object
            var expectedProfile = new PlayroomKit.Player.Profile
            {
                name = "PlayerName",
                photo = "player_photo_url",
                playerProfileColor = new PlayroomKit.Player.Profile.PlayerProfileColor
                {
                    r = 255,
                    g = 128,
                    b = 64,
                    hexString = "#FF8040",
                    hex = 16744448
                }
            };

            // Mock the GetProfileWrapper method to return the predefined JSON string
            _interop.GetProfileWrapper(testId).Returns(jsonString);


            // Act
            var result = _player.GetProfile();

            _interop.Received(1).GetProfileWrapper(testId);

            // Assert with error messages
            Assert.AreEqual(expectedProfile.name, result.name, "Profile name did not match the expected value.");
            Assert.AreEqual(expectedProfile.photo, result.photo, "Profile photo did not match the expected URL.");
            Assert.AreEqual(expectedProfile.playerProfileColor.r, result.playerProfileColor.r,
                "Player profile color red value did not match.");
            Assert.AreEqual(expectedProfile.playerProfileColor.g, result.playerProfileColor.g,
                "Player profile color green value did not match.");
            Assert.AreEqual(expectedProfile.playerProfileColor.b, result.playerProfileColor.b,
                "Player profile color blue value did not match.");
            Assert.AreEqual(expectedProfile.playerProfileColor.hexString, result.playerProfileColor.hexString,
                "Player profile color hex string did not match.");
            Assert.AreEqual(expectedProfile.playerProfileColor.hex, result.playerProfileColor.hex,
                "Player profile color hex value did not match.");
        }

        [Test]
        public void OnQuit_WhenInitialized_AddsCallbackAndReturnsUnsubscribeAction()
        {
            // Arrange
            Action<string> callback = msg => { };

            // Act
            var unsubscribe = _player.OnQuit(callback);

            // Assert
            Assert.IsNotNull(unsubscribe, "Expected unsubscribe action to be returned.");

            // Check that the callback is added to the list
            unsubscribe.Invoke(); // Calling unsubscribe should remove the callback

            // Internal check: Ensure callback list no longer contains the callback after unsubscribe
        }

        [Test]
        public void Kick_WhenInitialized_CallsKickPlayerWrapperWithCorrectParameters()
        {
            // Arrange
            Action onKickCallback = () => { };

            // Act
            _player.Kick(onKickCallback);

            // Assert
            _interop.Received(1).KickPlayerWrapper(testId, Arg.Any<Action>());
        }

        [Test]
        public void Kick_CallsInvokeKickInternal_CallbackIsInvoked()
        {
            // Arrange
            bool callbackInvoked = false;
            Action onKickCallback = () => { callbackInvoked = true; };


            // Set up the mock to invoke the callback when KickPlayerWrapper is called
            _interop.When(x => x.KickPlayerWrapper(
                    Arg.Any<string>(), // id
                    Arg.Any<Action>() // onKick callback
                ))
                .Do(callInfo =>
                {
                    var kickCallback = callInfo.ArgAt<Action>(1); // Correct index for the callback argument
                    kickCallback?.Invoke(); // Invoke the callback
                });

            //Act
            _player.Kick(onKickCallback);


            // Assert
            _interop.Received(1).KickPlayerWrapper(testId, Arg.Invoke());
            Assert.IsTrue(callbackInvoked, "Expected the onKickCallback to be invoked.");
        }

        [Test]
        public void WaitForState_WhenInitialized_CallsWaitForPlayerStateWrapperWithCorrectParameters()
        {
            // Arrange
            Action<string> onStateSetCallback = (data) => { Debug.Log("data: " + data); };

            // Act
            _player.WaitForState(testKey, onStateSetCallback);

            // Assert
            _interop.Received(1).WaitForPlayerStateWrapper(testId, testKey, onStateSetCallback);
        }
    }
}