using NUnit.Framework;
using System;
using UnityEngine;
using Playroom;
using NSubstitute;
using SimpleJSON;

public class PlayroomKitTests
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
        _mockPlayroomService = new PlayroomKit.PlayroomBuildService(_interop);
        _rpc = new PlayroomKit.RPC(_playroomKit, _interop);

        // Since PlayroomKit uses a private field for the service, we'll need to simulate it or test through the public API
        _playroomKit = new PlayroomKit(_mockPlayroomService, _rpc);
    }

    [Test]
    public void GetPlayer_ShouldReturnSamePlayer_ForSameId()
    {
        // Arrange
        string playerId = "Player1";

        // Act
        var player1 = _playroomKit.GetPlayer(playerId);
        var player2 = _playroomKit.GetPlayer(playerId);

        // Assert
        Assert.AreEqual(player1, player2, "GetPlayer should return the same instance for the same playerId.");
    }

    [Test]
    public void InternalInsertCoin_ShouldBeCalled()
    {
        var interopMock = Substitute.For<PlayroomKit.IInterop>();

        // No need to call DoNotCallBase() because it's an interface
        PlayroomKit.IPlayroomBase playroomService = new PlayroomKit.PlayroomBuildService(interopMock);

        PlayroomKit playroomKit = new PlayroomKit(playroomService, _rpc);

        playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() { { "score", 0 }, },
        }, () => { Debug.Log("Insert Callback Received!"); });

        interopMock.Received(1).InsertCoinWrapper(
            Arg.Is<string>(s =>
                s.Contains("\"maxPlayersPerRoom\":2") &&
                s.Contains(
                    "\"defaultPlayerStates\":{\"score\":0}")), // Check both maxPlayersPerRoom and defaultPlayerStates in the JSON
            Arg.Any<Action<string>>(), // Any Action<string> for the launch callback
            Arg.Any<Action<string>>(), // Any Action<string> for the quit handler
            Arg.Any<Action<string>>(), // Any Action<string> for the disconnect handler
            Arg.Any<Action<string>>(), // Any Action<string> for the error handler
            Arg.Any<string>(), // Any string for onLaunchCallBackKey
            Arg.Any<string>() // Any string for onDisconnectCallBackKey
        );
    }

    [Test]
    public void InternalInsertCoin_LaunchCallBackShouldBeInvoked()
    {
        var interopMock = Substitute.For<PlayroomKit.IInterop>();
        // Mocking the InsertCoinWrapper method behavior
        // Mocking the InsertCoinWrapper method behavior
        interopMock.When(x => x.InsertCoinWrapper(
                Arg.Any<string>(), // options JSON
                Arg.Any<Action<string>>(), // onLaunch callback
                Arg.Any<Action<string>>(), // onQuit callback
                Arg.Any<Action<string>>(), // onDisconnect callback
                Arg.Any<Action<string>>(), // onError callback
                Arg.Any<string>(), // onLaunch callback key
                Arg.Any<string>() // onDisconnect callback key
            ))
            .Do(callInfo =>
            {
                // Check if the callback is at the expected index and invoke it
                var onLaunchCallback = callInfo.ArgAt<Action<string>>(1); // Index 1 for onLaunch callback
                onLaunchCallback?.Invoke("onLaunchCallBack"); // Invoke the callback with a test key
            });

        PlayroomKit.IPlayroomBase playroomService = new PlayroomKit.PlayroomBuildService(interopMock);

        PlayroomKit playroomKit = new PlayroomKit(playroomService, _rpc);

        bool onLaunchInvoked = false;
        playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() { { "score", 0 }, },
        }, () => { onLaunchInvoked = true; });

        Assert.True(onLaunchInvoked);
    }

    [Test]
    public void GetPlayer_ShouldCreateNewPlayer_IfNotExists()
    {
        // Arrange
        string newPlayerId = "Player2";

        // Act
        var player = _playroomKit.GetPlayer(newPlayerId);

        // Assert
        Assert.IsNotNull(player, "GetPlayer should create a new Player if one does not exist.");
        Assert.AreEqual(newPlayerId, player.id, "The Player's Id should match the requested playerId.");
    }

    [Test]
    public void OnPlayerJoinBuild_ShouldInvokeCallback_WhenPlayerJoins()
    {
        // Arrange
        bool playerCreated = false;
        var playerId = "TestPlayerOPJ";
        PlayroomKit.Player testPlayer = null;
        _playroomKit.InsertCoin(new InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new() { { "score", 0 }, },
        }, () => { });

        _interop.When(x => x.OnPlayerJoinWrapper(
                Arg.Any<Action<string>>() // OnPlayerJoinCallback
            ))
            .Do(callInfo =>
            {
                // Check if the callback is at the expected index and invoke it
                var onPlayerJoin = callInfo.ArgAt<Action<string>>(0); // Index 1 for onLaunch callback
                onPlayerJoin?.Invoke(playerId); // Invoke the callback with a test key
            });


        // Act
        _playroomKit.OnPlayerJoin(player =>
        {
            testPlayer = player;
            Debug.Log("OnPlayerJoin player id: " + player.id);
            playerCreated = true;
        });

        var players = PlayroomKit.GetPlayers();

        // Assert
        Assert.IsTrue(playerCreated, "Callback should be invoked when player joins.");
        Assert.AreEqual(playerId, testPlayer.id, "The Player's Id should match the requested player.");
        Assert.Greater(players.Count, 0, "The players count should be greater than zero.");
    }

    [Test]
    public void GetRoomCode_ShouldInvoke_GetRoomCodeInternal()
    {
        var roomCode = _playroomKit.GetRoomCode();
        _interop.Received(1).GetRoomCodeWrapper();
    }

    [Test]
    public void StartMatchmaking_ShouldInvoke_StartMatchmakingInternal()
    {
        _playroomKit.StartMatchmaking();
        _interop.Received(1).StartMatchmakingWrapper(Arg.Any<Action>());
    }

    public class TestObject
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    [Test]
    public void SetState_ShouldInvokeCorrectInternalFunction_ForAllTypes()
    {
        // Act & Assert for int
        _playroomKit.SetState("TestState", 1); // For int
        _interop.Received(1).SetStateIntWrapper("TestState", 1, Arg.Any<bool>());

        // Act & Assert for float
        _playroomKit.SetState("TestState", 1.23f); // For float
        _interop.Received(1)
            .SetStateFloatWrapper("TestState", "1.23", Arg.Any<bool>()); // Assuming float is serialized as string

        // Act & Assert for string
        _playroomKit.SetState("TestState", "TestValue"); // For string
        _interop.Received(1).SetStateStringWrapper("TestState", "TestValue", Arg.Any<bool>());

        // Act & Assert for bool
        _playroomKit.SetState("TestState", true); // For bool
        _interop.Received(1).SetStateBoolWrapper("TestState", true, Arg.Any<bool>());

        // Arrange for JSON object
        var testObject = new TestObject { Name = "Test", Value = 100 }; // C# object
        var jsonString = JsonUtility.ToJson(testObject); // Serialize to JSON

        // Act & Assert for JSON object
        _playroomKit.SetState("TestState", testObject); // For JSON object
        _interop.Received(1).SetStateStringWrapper("TestState", jsonString, Arg.Any<bool>());
    }

    [Test]
    public void GetState_ShouldInvokeCorrectInternalFunction_ForAllTypes()
    {
        // Arrange & Act for int
        _interop.GetStateIntWrapper("TestState").Returns(42); // Mock for int
        var intValue = _playroomKit.GetState<int>("TestState");
        // Assert for int
        Assert.AreEqual(42, intValue);
        _interop.Received(1).GetStateIntWrapper("TestState");

        // Arrange & Act for float
        _interop.GetStateFloatWrapper("TestState").Returns(3.14f); // Mock for float
        var floatValue = _playroomKit.GetState<float>("TestState");
        // Assert for float
        Assert.AreEqual(3.14f, floatValue);
        _interop.Received(1).GetStateFloatWrapper("TestState");

        // Arrange & Act for bool
        _interop.GetStateIntWrapper("TestState").Returns(1); // Mock for bool
        var boolValue = _playroomKit.GetState<bool>("TestState");
        // Assert for bool
        Assert.IsTrue(boolValue);
        _interop.Received(2).GetStateIntWrapper("TestState");

        // Arrange & Act for string
        _interop.GetStateStringWrapper("TestState").Returns("TestValue"); // Mock for string
        var stringValue = _playroomKit.GetState<string>("TestState");
        // Assert for string
        Assert.AreEqual("TestValue", stringValue);
        _interop.Received(1).GetStateStringWrapper("TestState");

        // Arrange & Act for Vector2
        var vector2Json = JsonUtility.ToJson(new Vector2(1.0f, 2.0f));
        _interop.GetStateStringWrapper("TestState").Returns(vector2Json); // Mock for Vector2 as JSON
        var vector2Value = _playroomKit.GetState<Vector2>("TestState");
        // Assert for Vector2
        Assert.AreEqual(new Vector2(1.0f, 2.0f), vector2Value);
        _interop.Received(2).GetStateStringWrapper("TestState");

        // Arrange & Act for Vector3
        var vector3Json = JsonUtility.ToJson(new Vector3(1.0f, 2.0f, 3.0f));
        _interop.GetStateStringWrapper("TestState").Returns(vector3Json); // Mock for Vector3 as JSON
        var vector3Value = _playroomKit.GetState<Vector3>("TestState");
        // Assert for Vector3
        Assert.AreEqual(new Vector3(1.0f, 2.0f, 3.0f), vector3Value);
        _interop.Received(3).GetStateStringWrapper("TestState");

        // Arrange & Act for Vector4
        var vector4Json = JsonUtility.ToJson(new Vector4(1.0f, 2.0f, 3.0f, 4.0f));
        _interop.GetStateStringWrapper("TestState").Returns(vector4Json); // Mock for Vector4 as JSON
        var vector4Value = _playroomKit.GetState<Vector4>("TestState");
        // Assert for Vector4
        Assert.AreEqual(new Vector4(1.0f, 2.0f, 3.0f, 4.0f), vector4Value);
        _interop.Received(4).GetStateStringWrapper("TestState");

        // Arrange & Act for Quaternion
        var quaternionJson = JsonUtility.ToJson(new Quaternion(1.0f, 2.0f, 3.0f, 4.0f));
        _interop.GetStateStringWrapper("TestState").Returns(quaternionJson); // Mock for Quaternion as JSON
        var quaternionValue = _playroomKit.GetState<Quaternion>("TestState");
        // Assert for Quaternion
        Assert.AreEqual(new Quaternion(1.0f, 2.0f, 3.0f, 4.0f), quaternionValue);
        _interop.Received(5).GetStateStringWrapper("TestState");
    }


    [Test]
    public void RpcRegister_ShouldInvokeInternal_WhenCalled()
    {
        void HandleShoot(string data, string caller)
        {
            Debug.Log($"Caller: {caller}");
            Debug.Log("Shoot called!");
        }

        _playroomKit.RpcRegister("Shoot", HandleShoot, "You shot!");
        _interop.Received(1).RpcRegisterWrapper("Shoot", Arg.Any<Action<string>>(), "You shot!");
    }

    [Test]
    public void RpcCall_ShouldInvokeCallback_WhenCalled()
    {
        bool receivedCalled = false;
        _interop.When(x => x.RpcCallWrapper(
                Arg.Any<string>(),
                Arg.Any<string>(),
                PlayroomKit.RpcMode.ALL,
                Arg.Any<Action>()
            ))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<Action>(3); // Index 1 for onLaunch callback
                callback.Invoke();
            });

        _playroomKit.RpcCall("Shoot", "data is score", PlayroomKit.RpcMode.ALL, () => { receivedCalled = true; });

        UnityEngine.Assertions.Assert.IsTrue(receivedCalled);
    }

    [Test]
    public void IsStreamScreen_ShouldInvokeInternal_WhenCalled()
    {
        _playroomKit.IsStreamScreen();
        _interop.Received(1).IsStreamScreenWrapper();
    }

    [Test]
    public void WaitForState_ShouldInvokeInternal_WhenCalled()
    {
        void Callback2(string data)
        {
            Debug.Log($"Callback called!");
        }

        _playroomKit.WaitForState("state", Callback2);
        _interop.Received(1).WaitForStateWrapper("state", Arg.Any<Action<string, string>>());
    }


    [Test]
    public void WaitForPlayerState_ShouldInvokeInternal_WhenCalled()
    {
        void Callback(string data)
        {
            Debug.Log($"Callback called!: " + data);
        }

        var playerId = "1234";
        var state = "state";

        _playroomKit.WaitForPlayerState(playerId, state, Callback);
        _interop.Received(1).WaitForPlayerStateWrapper(playerId, state, Arg.Any<Action<string>>());
    }

    [Test]
    public void ResetStates_ShouldInvokeInternal_WhenCalled()
    {
        JSONArray CreateJsonArray(string[] array)
        {
            JSONArray jsonArray = new JSONArray();

            foreach (string item in array)
            {
                jsonArray.Add(item);
            }

            return jsonArray;
        }

        // Arrange
        var expectedKeysJson = CreateJsonArray(new[] { "pos" }).ToString(); // Assuming CreateJsonArray is available

        // Act
        _playroomKit.ResetStates(new[] { "pos" },
            () => { Debug.Log("Resetting Player states from Unity, Invoking from JS!"); });

        // Assert
        _interop.Received(1).ResetStatesWrapper(
            expectedKeysJson,
            Arg.Any<Action>() // Match any Action since the exact callback function may vary
        );
    }

    [Test]
    public void ResetPlayersStates_ShouldInvokeWrapperWithCorrectParams()
    {
        JSONArray CreateJsonArray(string[] array)
        {
            JSONArray jsonArray = new JSONArray();

            foreach (string item in array)
            {
                jsonArray.Add(item);
            }

            return jsonArray;
        }

        // Arrange
        var keysToExclude = new[] { "pos" };
        var expectedKeysJson = CreateJsonArray(keysToExclude).ToString(); // Assuming CreateJsonArray is available

        // Act
        _playroomKit.ResetPlayersStates(keysToExclude, () => { Debug.Log("Reset Player States are "); });

        // Assert
        _interop.Received(1).ResetPlayersStatesWrapper(
            expectedKeysJson,
            Arg.Any<Action>() // Verifying an Action was passed in, allowing flexibility
        );
    }


    [Test]
    public void CreateJoyStick_ShouldInvokeInteropWithCorrectJson()
    {
        // Arrange

        static string ConvertJoystickOptionsToJson(JoystickOptions options)
        {
            JSONNode joystickOptionsJson = new JSONObject();
            joystickOptionsJson["type"] = options.type;

            // Serialize the buttons array
            JSONArray buttonsArray = new JSONArray();
            foreach (ButtonOptions button in options.buttons)
            {
                JSONObject buttonJson = new JSONObject();
                buttonJson["id"] = button.id;
                buttonJson["label"] = button.label;
                buttonJson["icon"] = button.icon;
                buttonsArray.Add(buttonJson);
            }

            joystickOptionsJson["buttons"] = buttonsArray;

            // Serialize the zones property (assuming it's not null)
            if (options.zones != null)
            {
                JSONObject zonesJson = new JSONObject();
                zonesJson["up"] = ConvertButtonOptionsToJson(options.zones.up);
                zonesJson["down"] = ConvertButtonOptionsToJson(options.zones.down);
                zonesJson["left"] = ConvertButtonOptionsToJson(options.zones.left);
                zonesJson["right"] = ConvertButtonOptionsToJson(options.zones.right);
                joystickOptionsJson["zones"] = zonesJson;
            }

            return joystickOptionsJson.ToString();
        }

        // Function to convert ButtonOptions to JSON
        static JSONNode ConvertButtonOptionsToJson(ButtonOptions button)
        {
            JSONObject buttonJson = new JSONObject();
            buttonJson["id"] = button.id;
            buttonJson["label"] = button.label;
            buttonJson["icon"] = button.icon;
            return buttonJson;
        }

        var joystickOptions = new JoystickOptions
        {
            type = "dpad",
            buttons = new[]
            {
                new ButtonOptions { id = "btn1", label = "Jump", icon = "jump_icon" },
                new ButtonOptions { id = "btn2", label = "Run", icon = "run_icon" }
            },
            zones = new ZoneOptions
            {
                up = new ButtonOptions { id = "up", label = "Move Up", icon = "up_icon" },
                down = new ButtonOptions { id = "down", label = "Move Down", icon = "down_icon" },
                left = new ButtonOptions { id = "left", label = "Move Left", icon = "left_icon" },
                right = new ButtonOptions { id = "right", label = "Move Right", icon = "right_icon" }
            }
        };

        // Assuming ConvertJoystickOptionsToJson is either available or mocked
        var expectedJsonStr = ConvertJoystickOptionsToJson(joystickOptions);

        // Act
        _playroomKit.CreateJoyStick(joystickOptions);

        // Assert
        _interop.Received(1).CreateJoystickWrapper(expectedJsonStr);
    }

    [Test]
    public void MyPlayer_ShouldInvokeInternal()
    {
        _playroomKit.MyPlayer();
        _interop.Received(1).MyPlayerWrapper();
    }

    [Test]
    public void IsRunningInBrowser_ShouldReturnCorrectPlatform()
    {
        // Act & Assert
        bool isRunningInBrowser = PlayroomKit.IsRunningInBrowser();

#if UNITY_WEBGL && !UNITY_EDITOR
        Assert.IsTrue(isRunningInBrowser, "IsRunningInBrowser should return true when running in WebGL outside of the editor.");
#else
        Assert.IsFalse(isRunningInBrowser, "IsRunningInBrowser should return false when not running in WebGL.");
#endif
    }
}