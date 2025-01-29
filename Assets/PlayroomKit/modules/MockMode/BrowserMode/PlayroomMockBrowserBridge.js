InsertCoin = async function (options, onLaunchCallBackName, gameObjectName) {
  function onLaunchCallBack() {
    unityInstance.SendMessage(gameObjectName, onLaunchCallBackName);
  }

  await Playroom.insertCoin(options, onLaunchCallBack);
};

OnPlayerJoin = function (gameObjectName) {
  Playroom.onPlayerJoin((player) => {
    unityInstance.SendMessage(gameObjectName, "GetPlayerID", player.id);
    player.onQuit((state) => {
      unityInstance.SendMessage(gameObjectName, "OnQuitPlayer", player.id);
    });
  });
};

// States
SetState = function (key, value, reliable) {
  reliable = !!reliable;
  Playroom.setState(key, value, reliable);
};

GetState = function (key) {
  return JSON.stringify(Playroom.getState(key));
};

SetPlayerStateByPlayerId = function (playerId, key, value, reliable) {
  const players = Playroom.Multiplayer().getPlayers();

  reliable = !!reliable;

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }
  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  if (typeof playerState.setState === "function") {
    playerState.setState(key, value, reliable);
  } else {
    console.error('The player state object does not have a "setState" method.');
    return null;
  }
};

GetPlayerStateByPlayerId = function (playerId, key) {
  const players = Playroom.Multiplayer().getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }

  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  if (typeof playerState.getState === "function") {
    try {
      var stateVal = playerState.getState(key);

      if (stateVal === undefined) {
        return null;
      }

      return JSON.stringify(stateVal);
    } catch (error) {
      console.log("There was an error: " + error);
    }
  } else {
    console.error('The player state object does not have a "getState" method.');
    return null;
  }
};

GetRoomCode = function () {
  return Playroom.getRoomCode();
};

MyPlayer = function () {
  return Playroom.myPlayer().id;
};

IsHost = function () {
  return Playroom.isHost();
};

TransferHost = async function (playerId) {
  try {
    await transferHost(playerId);
  } catch (error) {
    console.error("Error transferring host:", error);
  }
};

IsStreamScreen = function () {
  return Playroom.isStreamScreen();
};

GetProfile = function (playerId) {
  const players = Playroom.Multiplayer().getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }

  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  if (typeof playerState.getProfile === "function") {
    const profile = playerState.getProfile();
    var returnStr = JSON.stringify(profile);

    return returnStr;
  } else {
    console.error(
      'The player state object does not have a "getProfile" method.'
    );
    return null;
  }
};

StartMatchmaking = async function () {
  await Playroom.startMatchmaking();
};

OnDisconnect = async function (callbackkey) {
  console.log("onDisconectCalled called", callbackkey);

  Playroom.onDisconnect((e) => {
    console.log(`Disconnected!`, e.code, e.reason, typeof e);
    unityInstance.SendMessage("CallbackManager", "InvokeCallback", callbackkey);
  });
};

WaitForState = function (stateKey, callbackKey) {
  Playroom.waitForState(stateKey)
    .then((stateVal) => {
      const data = {
        key: callbackKey,
        parameter: stateVal,
      };

      const jsonData = JSON.stringify(data);
      unityInstance.SendMessage("CallbackManager", "InvokeCallback", jsonData);
    })
    .catch((error) => {
      console.error("Error Waiting for state:", error);
    });
};

WaitForPlayerState = async function (playerId, stateKey, onStateSetCallback) {
  if (!window.Playroom) {
    console.error(
      "Playroom library is not loaded. Please make sure to call InsertCoin first."
    );
    reject("Playroom library not loaded");
    return;
  }

  const players = Playroom.Multiplayer().getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }
  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  Playroom.waitForPlayerState(playerState, stateKey).then((stateVal) => {
    const data = {
      key: onStateSetCallback,
      parameter: stateVal,
    };

    const jsonData = JSON.stringify(data);
    unityInstance.SendMessage("CallbackManager", "InvokeCallback", jsonData);
  });
};

Kick = async function (playerID) {
  if (!window.Playroom) {
    console.error(
      "Playroom library is not loaded. Please make sure to call InsertCoin first."
    );
    reject("Playroom library not loaded");
    return;
  }

  const players = Playroom.Multiplayer().getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }
  const playerState = players[playerID];

  if (!playerState) {
    console.error("Player with ID", playerID, "not found.");
    return null;
  }

  await playerState.kick();
};

ResetPlayersStates = async function (keysToExclude) {
  await Playroom.resetPlayersStates(keysToExclude);
};

ResetStates = async function (keysToExclude) {
  await Playroom.resetStates(keysToExclude);
};

//#region RPC
RpcRegister = function (name, callbackKey) {
  Playroom.RPC.register(name, (data, caller) => {
    const jsonData = {
      key: callbackKey,
      parameter: { data: data, callerId: caller.id },
    };

    const jsonString = JSON.stringify(jsonData);

    console.log(jsonString);

    unityInstance.SendMessage("CallbackManager", "HandleRPC", jsonString);
  });
};

RpcCall = function (name, data, rpcMode) {
  let mode;

  if (rpcMode === "ALL") {
    mode = Playroom.RPC.Mode.ALL;
  }

  if (rpcMode === "OTHERS") {
    mode = Playroom.RPC.Mode.OTHERS;
  }

  if (rpcMode === "HOST") {
    mode = Playroom.RPC.Mode.HOST;
  }

  Playroom.RPC.call(name, data, mode);
};
//#endregion

//#region Persistence
SetPersistentData = async function (key, value) {
  await Playroom.setPersistentData(key, value);
};

InsertPersistentData = async function (key, value) {
  await Playroom.insertPersistentData(key, value);
};

GetPersistentData = async function (key) {
  const data = await  Playroom.getPersistentData(key);
  return JSON.stringify(data)
};