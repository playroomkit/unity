using NUnit.Framework;
using NSubstitute; // For mocking the IPlayerService interface
using Playroom;

public class PlayerTests
{
    private PlayroomKit.Player _player;
    private IPlayerBase _mockPlayerService;

    [SetUp]
    public void SetUp()
    {
        // Mock the IPlayerService
        _mockPlayerService = Substitute.For<IPlayerBase>();

        // Create a new Player object with the mock service
        _player = new PlayroomKit.Player("TestPlayer", _mockPlayerService);
    }

    [Test]
    public void SetState_ShouldCallSetStateOnPlayerService_WithCorrectParameters()
    {
        // Arrange
        string key = "health";
        int value = 100;
        bool reliable = true;

        // Act
        _player.SetState(key, value, reliable);

        // Assert
        _mockPlayerService.Received(1).SetState(id: "test", key, value, reliable);
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
        _mockPlayerService.Received(1).SetState(id: "test", key, value, false);
    }

    [Test]
    public void GetState_ShouldReturnCorrectValue_FromPlayerService()
    {
        // Arrange
        string key = "health";
        int expectedValue = 100;

        // Mock the return value from GetState
        _mockPlayerService.GetState<int>(id: "test", key).Returns(expectedValue);

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
        _mockPlayerService.GetState<string>(id: "test", stringKey).Returns(expectedStringValue);
        _mockPlayerService.GetState<int>(id: "test", intKey).Returns(expectedIntValue);

        // Act
        string name = _player.GetState<string>(stringKey);
        int score = _player.GetState<int>(intKey);

        // Assert
        Assert.AreEqual(expectedStringValue, name, "GetState should return the correct string value.");
        Assert.AreEqual(expectedIntValue, score, "GetState should return the correct int value.");
    }
}