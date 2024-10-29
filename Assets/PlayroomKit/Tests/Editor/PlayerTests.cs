using NUnit.Framework;
using NSubstitute; // For mocking the IPlayerService interface
using Playroom;

public class PlayerTests
{
    private PlayroomKit _playroomKit;
    private PlayroomKit.Player _player;
    private PlayroomKit.Player.IPlayerBase _mockPlayerService;
    private PlayroomKit.IInterop _interop;

    [SetUp]
    public void SetUp()
    {
        _playroomKit = new PlayroomKit();
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() { {"score", 0}, },
        }, () =>
        {
            
        });
        _interop = Substitute.For<PlayroomKit.IInterop>();
        // Mock the IPlayerService
        _mockPlayerService = new PlayroomKit.Player.PlayerService(_interop);

        // Create a new Player object with the mock service
        _player = new PlayroomKit.Player("TestPlayer", _mockPlayerService);
        
    }

    [Test]
    public void SetState_ShouldCallSetStateOnPlayerService_WithCorrectParameters()
    {
        // Arrange
        string key = "health";
        int value = 100;
        bool reliable = false;

        // Act
        _player.SetState(key, value, reliable);

        // Assert
        _interop.Received(1).SetPlayerStateIntWrapper(_player.id, key, value, false);
    }

    [Test]
    public void SetState_ShouldUseDefaultReliableParameter_WhenNotProvided()
    {
        // Arrange
        string key = "armor";
        int value = 50;

        // Act
        _player.SetState(key, value); // Do not specify reliable

        // Assert
        _interop.Received(1).SetPlayerStateIntWrapper(_player.id, key, value, false);
    }

    [Test]
    public void GetState_ShouldReturnCorrectValue_FromPlayerService()
    {
        // Arrange
        string key = "health";
        int expectedValue = 100;

        // Mock the return value from GetState
        _mockPlayerService.GetState<int>(_player.id, key).Returns(expectedValue);

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
        _mockPlayerService.GetState<string>(_player.id, stringKey).Returns(expectedStringValue);
        _mockPlayerService.GetState<int>(_player.id, intKey).Returns(expectedIntValue);

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
        // Arrange
        string testId = "TestPlayer";
        // Mock JSON string that GetProfileWrapper would return
        string jsonString = "{\"name\": \"PlayerName\", \"photo\": \"player_photo_url\", \"color\": { \"r\": 255, \"g\": 128, \"b\": 64, \"hexString\": \"#FF8040\", \"hex\": 16744448 }}";
            
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
        
        _interop.Received(1).GetProfileWrapper(testId);

        // Act
        var result = _player.GetProfile();
        
        // Assert with error messages
        Assert.AreEqual(expectedProfile.name, result.name, "Profile name did not match the expected value.");
        Assert.AreEqual(expectedProfile.photo, result.photo, "Profile photo did not match the expected URL.");
        Assert.AreEqual(expectedProfile.playerProfileColor.r, result.playerProfileColor.r, "Player profile color red value did not match.");
        Assert.AreEqual(expectedProfile.playerProfileColor.g, result.playerProfileColor.g, "Player profile color green value did not match.");
        Assert.AreEqual(expectedProfile.playerProfileColor.b, result.playerProfileColor.b, "Player profile color blue value did not match.");
        Assert.AreEqual(expectedProfile.playerProfileColor.hexString, result.playerProfileColor.hexString, "Player profile color hex string did not match.");
        Assert.AreEqual(expectedProfile.playerProfileColor.hex, result.playerProfileColor.hex, "Player profile color hex value did not match.");
    }
}