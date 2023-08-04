mergeInto(LibraryManager.library, {
  /**
   * @description Inserts a coin into the game by loading the required scripts and initializing the Playroom.
   * @param {function} callback - A callback function to execute after the Playroom is loaded.
   */
  InsertCoin: function (callback) {
    function embedScript(src) {
      return new Promise((resolve, reject) => {
        var script = document.createElement("script");
        script.src = src;
        script.async = false;
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
      });
    }

    Promise.all([
      embedScript("https://unpkg.com/react@18.2.0/umd/react.development.js"),
      embedScript("https://unpkg.com/react-dom/umd/react-dom.development.js"),
      embedScript("https://unpkg.com/playroomkit/multiplayer.umd.js"),
    ])
      .then(() => {
        if (!window.Playroom) {
          console.error(
            "Playroom library is not loaded. Please make sure to call InsertCoin first."
          );
          return;
        }

        console.info("Playroom has loaded.");

        Playroom.insertCoin()
          .then(() => {
            dynCall("v", callback, []);
          })
          .catch((error) => {
            console.error("Error inserting coin:", error);
          });
      })
      .catch((error) => {
        console.error("Error loading Playroom:", error);
      });
  },

  /**
   * @description Checks whether the player is the host of the game.
   * @returns {boolean} True if the local player is the host, otherwise false.
   */
  IsHost: function () {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    return Playroom.isHost();
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

    Playroom.onPlayerJoin((player) => {
      // Call the C# callback function with the player.id as a string parameter
      var id = player.id;
      var bufferSize = lengthBytesUTF8(id) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(id, buffer, bufferSize);
      dynCall("vi", functionPtr, [buffer]);
    });
  },

 
  /* ----- MULTIPLAYER GETTERS AND SETTERS  ↓ ----- */

  /**
   * @description Sets a key-value pair in the game state with a numerical or boolean value.
   * @param {string} key - The key to set in the game state.
   * @param {number | boolean} value - The value to associate with the key, such as position or health.
   */
  SetState: function (key, value) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    Playroom.setState(UTF8ToString(key), value);
  },

  /**
   * @description Sets a key-value pair in the game state with a string value.
   * @param {string} key - The key to set in the game state.
   * @param {string} stringVal - The string value to associate with the key.
   */
  SetStateString: function (key, stringVal) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    Playroom.setState(UTF8ToString(key), UTF8ToString(stringVal));
  },

  /**
   * @description Sets a key-value pair in the game state with a dictionary (JSON) value.
   * @param {string} key - The key to set in the game state.
   * @param {string} jsonValues - The JSON representation of the dictionary value to associate with the key.
   */
  SetStateDictionary: function (key, jsonValues) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    Playroom.setState(UTF8ToString(key), JSON.parse(UTF8ToString(jsonValues)));
  },

  /**
   * @description Retrieves an integer value from the game state using the provided key.
   * @param {string} key - The key to retrieve the integer value from the game state.
   * @returns {number | null} The integer value associated with the key, or null if the key is not found.
   */
  GetStateInt: function (key) {
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
   * @returns {number | null} The floating-point value associated with the key, or null if the key is not found.
   */
  GetStateFloat: function (key) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    var flt = Playroom.getState(UTF8ToString(key));
    return flt;
  },

  /**
   * @description Retrieves a string value from the game state using the provided key.
   * @param {string} key - The key to retrieve the string value from the game state.
   * @returns {string | null} The string value associated with the key, or null if the key is not found.
   */
  GetStateString: function (key) {
    if (!window.Playroom) {
      console.error(
        "Playroom library is not loaded. Please make sure to call InsertCoin first."
      );
      return;
    }
    var returnStr = Playroom.getState(UTF8ToString(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  /**
   * @description Retrieves a dictionary (JSON) value from the game state using the provided key.
   * @param {string} key - The key to retrieve the dictionary value from the game state.
   * @returns {string | null} The JSON representation of the dictionary value associated with the key, or null if the key is not found.
   */
  GetStateDictionary: function (key) {
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
    const players = window._multiplayer.getPlayers();

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

      // currently sending only the color only!
      var returnStr = profile.color.hexString;
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
  SetPlayerStateByPlayerId: function (playerId, key, value) {
    const players = window._multiplayer.getPlayers();

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    // Assuming that the player state object has a "setState" method
    if (typeof playerState.setState === "function") {
      playerState.setState(UTF8ToString(key), value);
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
  SetPlayerStateStringById: function (playerId, key, value) {
    const players = window._multiplayer.getPlayers();

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
      playerState.setState(UTF8ToString(key), UTF8ToString(value));
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
  SetPlayerStateDictionary: function (playerId, key, jsonValues) {
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
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
        JSON.parse(UTF8ToString(jsonValues))
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
    const players = window._multiplayer.getPlayers();

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
    const players = window._multiplayer.getPlayers();

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
    const players = window._multiplayer.getPlayers();

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
      var bufferSize = lengthBytesUTF8(stateVal) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(stateVal, buffer, bufferSize);
      return buffer;
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
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    // Assuming that the player state object has a "setState" method
    if (typeof playerState.setState === "function") {
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

  //Player Quit:  

  OnQuitInternal: function(playerId, callback) {
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error("Player with ID", UTF8ToString(playerId), "not found.");
      return null;
    }

    // Assuming that the player state object has a "setState" method
    if (typeof playerState.onQuit === "function") {
      playerState.onQuit(() => {
        dynCall("v", callback, []);
      });
    } else {
      console.error(
        'The player state object does not have a "setState" method.'
      );
      return null;
    }
  }


});
