mergeInto(LibraryManager.library, {
  /**
   * @description Inserts a coin into the game by loading the required scripts and initializing the Playroom.
   * @param {function} onLaunchCallBack - A callback function to execute after the Playroom is loaded.
   * @param {function} onQuitInternalCallback - (internal) This C# callback function calls an OnQuit wrapper on C# side, with the player's ID.
   */
  InsertCoinInternal: function (
    optionsJson,
    onLaunchCallBack,
    onQuitInternalCallback,
    onDisconnectCallback,
    onError,
    onLaunchCallBackKey,
    onQuitInternalCallbackKey
  ) {
    onLaunchCallBackKey = UTF8ToString(onLaunchCallBackKey);
    onQuitInternalCallbackKey = UTF8ToString(onQuitInternalCallbackKey);


    function OnLaunchCallBack() {
      var key = _ConvertString(onLaunchCallBackKey);
      {{{ makeDynCall('vi', 'onLaunchCallBack') }}}(key)
    }

    function OnDisconnectCallback() {
      var key = _ConvertString(onQuitInternalCallbackKey);
      {{{ makeDynCall('vi', 'onDisconnectCallback') }}}(key)
    }
    this.onPlayerJoinCallBacks = {};
    var options = optionsJson ? JSON.parse(UTF8ToString(optionsJson)) : {};

    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    Playroom.insertCoin(options, OnLaunchCallBack, OnDisconnectCallback)
      .then(() => {
        Playroom.onPlayerJoin((player) => {
          var id = player.id;
          var bufferSize = lengthBytesUTF8(id) + 1;
          var buffer = _malloc(bufferSize);
          stringToUTF8(id, buffer, bufferSize);

          player.onQuit(() => {
            {{{ makeDynCall('vi', 'onQuitInternalCallback') }}}(buffer)
          });
        });
      })
      .catch((error) => {
        var jsonString = JSON.stringify(error);
        var bufferSize = lengthBytesUTF8(jsonString) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(jsonString, buffer, bufferSize);
        {{{ makeDynCall('vi','onError') }}}(buffer)
      });
  },

  /**
   * @description Checks whether the player is the host of the game.
   * @returns {boolean} True if the local player is the host, otherwise false.
   */
  IsHostInternal: function () {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    return Playroom.isHost();
  },

  /**
   * @description Transfers the host to another player if they are in the room
   * @param {string} playerId 
   */
  TransferHostInternal: function (playerId) {
    if (!window.Playroom) {
      console.error("Playroom library is not loaded. Please make sure to call InsertCoin first.");
      return;
    }

    try {
      Playroom.transferHost(playerId)
        .then(() => {
          console.log("Host privileges successfully transferred.");
        })
        .catch((error) => {
          console.error("Failed to transfer host privileges: ", error);
        });
    } catch (error) {
      console.error("Error transferring host: ", error);
    }
  },

  /**
   * @description Checks whether the local game is running in stream mode.
   * @returns {boolean} True if the local game is running in stream mode, otherwise false.
   */
  IsStreamScreenInternal: function () {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    return Playroom.isStreamScreen();
  },

  /**
   * @description Retrieves the current local player.
   * @returns {string} The current player's ID.
   */
  MyPlayerInternal: function () {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    var myPlayerID = Playroom.myPlayer().id;
    var bufferSize = lengthBytesUTF8(myPlayerID) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(myPlayerID, buffer, bufferSize);
    return buffer;
  },

  MeInternal: function () {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    var myPlayerID = Playroom.me().id;
    var bufferSize = lengthBytesUTF8(myPlayerID) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(myPlayerID, buffer, bufferSize);
    return buffer;
  },

  /**
   * @description Registers a callback to be executed when a new player joins the game.
   * @param {function} functionPtr - A C# callback function that receives the player's ID as a string parameter.
   */

  OnPlayerJoinInternal: function (functionPtr) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    var callbackID = Date.now().toString();
    try {
      var unsubcribePlayerJoin = Playroom.onPlayerJoin((player) => {
        var id = player.id;
        var bufferSize = lengthBytesUTF8(id) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(id, buffer, bufferSize);
        
        {{{ makeDynCall('vi', 'functionPtr') }}}(buffer)
      });
    } catch (error) {
      console.log(error);
    }

    this.onPlayerJoinCallBacks[callbackID] = unsubcribePlayerJoin;
    var callbackIDbufferSize = lengthBytesUTF8(callbackID) + 1;
    var callbackIDUTF8 = _malloc(callbackIDbufferSize);
    stringToUTF8(callbackID, callbackIDUTF8, callbackIDbufferSize);
    return callbackIDUTF8;
  },

  UnsubscribeOnPlayerJoinInternal: function (id) {
    functionId = UTF8ToString(id);
    var unsubscribeFunction = this.onPlayerJoinCallBacks[functionId];
    if (unsubscribeFunction) {
      unsubscribeFunction();
      delete this.onPlayerJoinCallBacks[functionId];
    } else {
      console.error(
        "No player join event handler with ID " + functionId + " to unregister."
      );
    }
  },

  UnsubscribeOnQuitInternal: function () {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    if (this.unsubscribeOnQuit) {
      this.unsubscribeOnQuit();
    } else {
      console.error("No On Quit event handler to unregister.");
    }
  },

  /* ----- MULTIPLAYER GETTERS AND SETTERS  ↓ ----- */

  /**
   * @description Sets a key-value pair in the game state with a numerical or boolean value.
   * @param {string} key - The key to set in the game state.
   * @param {number | boolean} value - The value to associate with the key, such as position or health.
   */
  SetStateInternal: function (key, value, reliable) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    reliable = !!reliable;

    Playroom.setState(UTF8ToString(key), value, reliable);
  },

  /**
   * @description Sets a key-value pair in the game state with a float value.
   * @param {string} key - The key to set in the game state.
   * @param {number | boolean} value - The value to associate with the key, such as position or health.
   */
  SetStateFloatInternal: function (key, value, reliable) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    reliable = !!reliable;

    Playroom.setState(
      UTF8ToString(key),
      parseFloat(UTF8ToString(value)),
      reliable
    );
  },

  /**
   * @description Sets a key-value pair in the game state with a string value.
   * @param {string} key - The key to set in the game state.
   * @param {string} stringVal - The string value to associate with the key.
   */
  SetStateString: function (key, stringVal, reliable) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    reliable = !!reliable;

    Playroom.setState(UTF8ToString(key), UTF8ToString(stringVal), reliable);
  },

  /**
   * @description Sets a key-value pair in the game state with a dictionary (JSON) value.
   * @param {string} key - The key to set in the game state.
   * @param {string} jsonValues - The JSON representation of the dictionary value to associate with the key.
   */
  SetStateDictionary: function (key, jsonValues, reliable) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    reliable = !!reliable;

    Playroom.setState(
      UTF8ToString(key),
      JSON.parse(UTF8ToString(jsonValues)),
      reliable
    );
  },

  /**
   * @description Retrieves an integer value from the game state using the provided key.
   * @param {string} key - The key to retrieve the integer value from the game state.
   * @returns {number | null} The integer value associated with the key, or null if the key is not found.
   */
  GetStateIntInternal: function (key) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    return Playroom.getState(UTF8ToString(key));
  },

  /**
   * @description Retrieves a floating-point value from the game state using the provided key.
   * @param {string} key - The key to retrieve the floating-point value from the game state.
   * @returns {string | null} The floating-point value associated with the key, or null if the key is not found.
   */
  GetStateFloatInternal: function (key) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    return Playroom.getState(UTF8ToString(key));
  },

  /**
   * @description Retrieves a string value from the game state using the provided key.
   * @param {string} key - The key to retrieve the string value from the game state.
   * @returns {string | null} The string value associated with the key, or null if the key is not found.
   */
  GetStateStringInternal: function (key) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
    }

    try {
      var returnStr = Playroom.getState(UTF8ToString(key));

      if (returnStr == null) {
        return "";
      }

      if (typeof returnStr !== "string") {
        return "";
      }

      var bufferSize = lengthBytesUTF8(returnStr) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(returnStr, buffer, bufferSize);
      return buffer;
    } catch (error) {
      console.error(
        "JavaScript Library: An error occurred in GetStateStringInternal: \n\n",
        error
      );
    }
  },

  /**
   * @description Retrieves a dictionary (JSON) value from the game state using the provided key.
   * @param {string} key - The key to retrieve the dictionary value from the game state.
   * @returns {string | null} The JSON representation of the dictionary value associated with the key, or null if the key is not found.
   */
  GetStateDictionaryInternal: function (key) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    var obj = Playroom.getState(UTF8ToString(key));
    var jsonString = JSON.stringify(obj);
    var bufferSize = lengthBytesUTF8(jsonString) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },

  /* ----- PLAYERSTATE GETTERS AND SETTERS ↓ ----- */

  /**
   * @description Retrieves a player's profile color by their player ID.
   * @param {string} playerId - The ID of the player whose profile color to retrieve.
   * @returns {string | null} return the hexColor for the player's profile color, or null if the player is not found.
   */
  GetProfileByPlayerId: function (playerId) {
    const players = Playroom.Multiplayer().getPlayers()

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.getProfile === "function") {
      const profile = playerState.getProfile();

      var returnStr = JSON.stringify(profile);

      var bufferSize = lengthBytesUTF8(returnStr) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(returnStr, buffer, bufferSize);
      return buffer;
    } else {
      console.error(
        'The player state object does not have a "getProfile" method.'
      );
      return null;
    }
  },

  /**
   * @description Sets a key-value pair in a specific player's state with a numerical or boolean value.
   * @param {string} playerId - The ID of the player whose state to update.
   * @param {string} key - The key to set in the player's state.
   * @param {number | boolean} value - The value to associate with the key in the player's state.
   */
  SetPlayerStateByPlayerId: function (playerId, key, value, reliable) {
    const players = Playroom.Multiplayer().getPlayers()

    reliable = !!reliable;

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.setState === "function") {
      playerState.setState(UTF8ToString(key), value, reliable);
    } else {
      console.error(
        'The player state object does not have a "setState" method.'
      );
      return null;
    }
  },

  SetPlayerStateFloatByPlayerId: function (playerId, key, value, reliable) {
    const players = Playroom.Multiplayer().getPlayers()

    reliable = !!reliable;

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.setState === "function") {
      playerState.setState(
        UTF8ToString(key),
        parseFloat(UTF8ToString(value)),
        reliable
      );
    } else {
      console.error(
        'The player state object does not have a "setState" method.'
      );
      return null;
    }
  },

  /**
   * @description Sets a key-value pair in a specific player's state with a string value.
   * @param {string} playerId - The ID of the player whose state to update.
   * @param {string} key - The key to set in the player's state.
   * @param {string} value - The string value to associate with the key in the player's state.
   */
  SetPlayerStateStringById: function (playerId, key, value, reliable) {
    const players = Playroom.Multiplayer().getPlayers()

    reliable = !!reliable;

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.setState === "function") {
      playerState.setState(UTF8ToString(key), UTF8ToString(value), reliable);
    } else {
      console.error(
        'The player state object does not have a "setState" method.'
      );
      return null;
    }
  },

  /**
   * @description Sets a key-value pair in a specific player's state with a dictionary (JSON) value.
   * @param {string} playerId - The ID of the player whose state to update.
   * @param {string} key - The key to set in the player's state.
   * @param {string} jsonValues - The JSON representation of the dictionary value to associate with the key in the player's state.
   */
  SetPlayerStateDictionary: function (playerId, key, jsonValues, reliable) {
    const players = Playroom.Multiplayer().getPlayers()

    reliable = !!reliable;
    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.setState === "function") {
      playerState.setState(
        UTF8ToString(key),
        JSON.parse(UTF8ToString(jsonValues)),
        reliable
      );
    } else {
      console.error(
        'The player state object does not have a "setState" method.'
      );
      return null;
    }
  },

  /**
   * @description Retrieves an integer value from a specific player's state using the provided key.
   * @param {string} playerId - The ID of the player whose state to query.
   * @param {string} key - The key to retrieve the integer value from the player's state.
   * @returns {number | null} The integer value associated with the key in the player's state, or null if the player is not found or the key is not found in the player's state.
   */
  GetPlayerStateIntById: function (playerId, key) {
    const players = Playroom.Multiplayer().getPlayers()

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.getState === "function") {
      var stateVal = playerState.getState(UTF8ToString(key));
      return stateVal;
    } else {
      console.error(
        'The player state object does not have a "getState" method.'
      );
      return null;
    }
  },

  /**
   * @description Retrieves a floating-point value from a specific player's state using the provided key.
   * @param {string} playerId - The ID of the player whose state to query.
   * @param {string} key - The key to retrieve the floating-point value from the player's state.
   * @returns {number | null} The floating-point value associated with the key in the player's state, or null if the player is not found or the key is not found in the player's state.
   */
  GetPlayerStateFloatById: function (playerId, key) {
    const players = Playroom.Multiplayer().getPlayers()

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.getState === "function") {
      var stateVal = playerState.getState(UTF8ToString(key));
      return stateVal;
    } else {
      console.error(
        'The player state object does not have a "getState" method.'
      );
      return null;
    }
  },

  /**
   * @description Retrieves a string value from a specific player's state using the provided key.
   * @param {string} playerId - The ID of the player whose state to query.
   * @param {string} key - The key to retrieve the string value from the player's state.
   * @returns {string | null} The string value associated with the key in the player's state, or null if the player is not found or the key is not found in the player's state.
   */
  GetPlayerStateStringById: function (playerId, key) {
    const players = Playroom.Multiplayer().getPlayers()

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.getState === "function") {
      try {
        var stateVal = playerState.getState(UTF8ToString(key));

        if (stateVal === undefined) {
          return null;
        }

        var bufferSize = lengthBytesUTF8(stateVal) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(stateVal, buffer, bufferSize);
        return buffer;
      } catch (error) {
        console.log("There was an error: " + error);
      }
    } else {
      console.error(
        'The player state object does not have a "getState" method.'
      );
      return null;
    }
  },

  /**
   * @description Retrieves a dictionary (JSON) value from a specific player's state using the provided key.
   * @param {string} playerId - The ID of the player whose state to query.
   * @param {string} key - The key to retrieve the dictionary value from the player's state.
   * @returns {string | null} The JSON representation of the dictionary value associated with the key in the player's state, or null if the player is not found or the key is not found in the player's state.
   */
  GetPlayerStateDictionary: function (playerId, key) {
    const players = Playroom.Multiplayer().getPlayers()

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    if (typeof playerState.getState === "function") {
      var obj = playerState.getState(UTF8ToString(key));
      var jsonString = JSON.stringify(obj);
      var bufferSize = lengthBytesUTF8(jsonString) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(jsonString, buffer, bufferSize);
      return buffer;
    } else {
      console.error(
        'The player state object does not have a "setState" method.'
      );
      return null;
    }
  },

  CreateJoystickInternal: function (JoystickOptions) {
    const options = JoystickOptions
      ? JSON.parse(UTF8ToString(JoystickOptions))
      : {};

    this.leftStick = new Playroom.Joystick(Playroom.myPlayer(), {
      type: options.type,
      buttons: options.buttons,
      zones: options.zones,
    });
  },

  DpadJoystickInternal: function () {
    const dpad = this.leftStick.dpad();

    var jsonString = JSON.stringify(dpad);
    var bufferSize = lengthBytesUTF8(jsonString) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },

  GetRoomCodeInternal: function () {
    var roomCode = Playroom.getRoomCode();
    var bufferSize = lengthBytesUTF8(roomCode) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(roomCode, buffer, bufferSize);
    return buffer;
  },

  OnDisconnectInternal: function (callback) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    Playroom.onDisconnect((e) => {
      console.log(`Disconnected!`, e.code, e.reason);
      {{{ makeDynCall('v', 'callback') }}}()
    });
  },

  WaitForStateInternal: function (stateKey, onStateSetCallback) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      reject("Playroom library not loaded");
      return;
    }

    stateKey = UTF8ToString(stateKey);
    Playroom.waitForState(stateKey)
      .then((stateVal) => {
        stateVal = JSON.stringify(stateVal);

        var key = _ConvertString(stateKey);
        
        {{{ makeDynCall('vii', 'onStateSetCallback') }}}(key, stringToNewUTF8(stateVal))
      })
      .catch((error) => {
        console.error("Error Waiting for state:", error);
      });
  },

  WaitForPlayerStateInternal: function (
    playerId,
    stateKey,
    onStateSetCallback
  ) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      reject("Playroom library not loaded");
      return;
    }

    const players = Playroom.Multiplayer().getPlayers()

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    stateKey = UTF8ToString(stateKey);
    Playroom.waitForPlayerState(playerState, stateKey)
      .then((stateVal) => {
        {{{ makeDynCall('vi', 'onStateSetCallback') }}}(stringToNewUTF8(stateVal))
      })
      .catch((error) => {
        console.error("Error waiting for state:", error);
      });
  },

  KickInternal: function (playerID, onKickCallBack) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      reject("Playroom library not loaded");
      return;
    }

    const players = Playroom.Multiplayer().getPlayers()

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }
    const playerState = players[UTF8ToString(playerID)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerID), "not found.");
      return null;
    }

    playerState
      .kick()
      .then(() => {
        {{{ makeDynCall('v', 'onKickCallback') }}}()
      })
      .catch((error) => {
        console.error("Error kicking player:", error);
      });
  },

  ResetStatesInternal: function (keysToExclude, onStatesReset) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    var keys = keysToExclude ? JSON.parse(UTF8ToString(keysToExclude)) : [];
    Playroom.resetStates(keys)
      .then(() => {
        {{{ makeDynCall('v', 'onStatesReset') }}}()
      })
      .catch((error) => {
        console.error("Error resetting states:", error);
        throw error;
      });
  },

  ResetPlayersStatesInternal: function (keysToExclude, onStatesReset) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    var keys = keysToExclude ? JSON.parse(UTF8ToString(keysToExclude)) : [];
    Playroom.resetPlayersStates(keys)
      .then(() => {
        {{{ makeDynCall('v', 'onStatesReset') }}}()
      })
      .catch((error) => {
        console.error("Error resetting players states:", error);
        throw error;
      });
  },

  RpcRegisterInternal: function (name, callback, onResponseReturn) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    var n = UTF8ToString(name)
    onResponseReturn = UTF8ToString(onResponseReturn);

    function registerCallback(data, sender) {
      var combinedData = {
        data: data,
        senderId: sender.id,
        eventName : n
      };

      var dataJson = JSON.stringify(combinedData);

      {{{ makeDynCall('vi', 'callback') }}}(stringToNewUTF8(dataJson));

      return onResponseReturn;
    }

    Playroom.RPC.register(UTF8ToString(name), registerCallback);
  },

  RpcCallInternal: function (name, dataJson, mode, callbackOnResponse) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    try {
      var data;
      if (dataJson) {
        try {
          data = JSON.parse(UTF8ToString(dataJson));
        } catch (parseError) {
          data = UTF8ToString(dataJson);
        }
      } else {
        data = {};
      }

      function onResponseCallback(responseData) {
        // console.log("Response received: ", responseData);
        {{{ makeDynCall('v', 'callbackOnResponse') }}}()
      }

      Playroom.RPC.call(UTF8ToString(name), data, mode, onResponseCallback);
    } catch (error) {
      console.error("Error in RpcCallInternal:", error);
    }
  },

  StartMatchmakingInternal: function (callback) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    Playroom.startMatchmaking()
      .then(() => {
        console.log(`Player has joined a public room`);
        {{{ makeDynCall('v', 'callback') }}}()
      })
      .catch((error) => {
        console.error(`JS: Error starting match making ${error}`);
      });
  },
  
   //#region Persistence
   SetPersistentDataInternal: function (key, value) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    
    Playroom.setPersistentData(UTF8ToString(key), UTF8ToString(value)).then(() => {
      console.log("Data has been set successfully.");
    }).catch((error) => {
      console.error("Failed to set data:", error);
    });
  },
 
  InsertPersistentDataInternal: function (key, value) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    
    Playroom.insertPersistentData(UTF8ToString(key), UTF8ToString(value)).then(() => {
      console.log("Data has been set successfully.");
    }).catch((error) => {
      console.error("Failed to set data:", error);
    });
  },
  
  GetPersistentDataInternal: function (key, onGetPersistentDataCallback) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }

    var dataKey = UTF8ToString(key);

    Playroom.getPersistentData(dataKey).then(data => {
      if (data === undefined) {
        return null;
      }

      data = JSON.stringify(data);
      var key = _ConvertString(dataKey);
      {{{ makeDynCall('vii', 'onGetPersistentDataCallback') }}}(key, stringToNewUTF8(data))
    }).catch((error) => {
      console.error("Error getting persistent data:", error);
    });
  
  },
  //#endregion

  // UTILS
  /**
   * Converts a given string into a UTF-8 encoded string and stores it in memory.
   *
   * @param {string} str - The string to be converted.
   * @returns {number} The memory address of the buffer where the converted string is stored.
   */
  ConvertString: function (str) {
    var bufferSize = lengthBytesUTF8(str) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(str, buffer, bufferSize);
    return buffer;
  },
});
