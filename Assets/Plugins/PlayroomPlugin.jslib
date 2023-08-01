mergeInto(LibraryManager.library, {

  InsertCoin: function (callback) {
    function embedScript(src) {
      return new Promise((resolve, reject) => {
        var script = document.createElement('script');
        script.src = src;
        script.async = false;
        script.onload = resolve; // Resolve the promise when the script is loaded
        script.onerror = reject; // Reject the promise if the script fails to load
        document.head.appendChild(script);
      });
    };

    // Load all the CDNs using Promise.all
    Promise.all([
      embedScript('https://unpkg.com/react@18.2.0/umd/react.development.js'),
      embedScript('https://unpkg.com/react-dom/umd/react-dom.development.js'),
      embedScript('https://unpkg.com/playroomkit/multiplayer.umd.js')
    ]).then(() => {


      console.log('All CDNs have been loaded');

      if (!window.Playroom) {
        console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
        return;
      }

      Playroom.insertCoin().then(() => {

        dynCall("v", callback, []);

      }).catch((error) => {
        console.error('Error inserting coin:', error);
      });


    }).catch((error) => {
      console.error('Error loading CDNs:', error);
    });
  },




  // for numbers / bools
  SetState: function (key, value) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }
    console.log("key", UTF8ToString(key), "value", value)
    Playroom.setState(UTF8ToString(key), value);
  },

  SetStateString: function (key, stringVal) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }
    console.log("key", UTF8ToString(key), "value", UTF8ToString(stringVal))
    Playroom.setState(UTF8ToString(key), UTF8ToString(stringVal));
  },


  // for numbers
  GetStateInt: function (key) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }

    return Playroom.getState(UTF8ToString(key));
  },

  GetStateFloat: function (key) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }


    console.log("float coming from unity: ", Playroom.getState(UTF8ToString(key)));

    var flt = Playroom.getState(UTF8ToString(key))
    return flt;
  },

  GetStateString: function (key) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }


    // retrun value to unity
    var returnStr = Playroom.getState(UTF8ToString(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;

  },

  SetStateDictionary: function (key, jsonValues) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }

    console.log("jsonValues from unity: ", UTF8ToString(jsonValues))
    playerState.set(UTF8ToString(key), JSON.parse(UTF8ToString(jsonValues)));


    // TODO: implement GetStateDictionary 


  },



  IsHost: function () {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }

    // retrun value to unity
    return Playroom.isHost();
  },

  OnPlayerJoinInternal: function (functionPtr) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
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


  GetProfileByPlayerId: function (playerId) {
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
    if (typeof players !== 'object' || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error('Player with ID', UTF8ToString(playerId), 'not found.');
      return null;
    }

    if (typeof playerState.getProfile === 'function') {
      const profile = playerState.getProfile();
      console.log("Log profile: ", profile);

      // currently sending only the color
      var returnStr = profile.color.hexString;
      var bufferSize = lengthBytesUTF8(returnStr) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(returnStr, buffer, bufferSize);
      return buffer;

    } else {
      console.error('The player state object does not have a "getProfile" method.');
      return null;
    }
  },

  // for int / float(issues with float type) / bool
  SetPlayerStateByPlayerId: function (playerId, key, value) {
    const players = window._multiplayer.getPlayers();

    if (typeof players !== 'object' || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error('Player with ID', UTF8ToString(playerId), 'not found.');
      return null;
    }

    // Assuming that the player state object has a "getProfile" method
    if (typeof playerState.setState === 'function') {
      playerState.setState(UTF8ToString(key), value);
      console.log(`setting state: ${UTF8ToString(key)} for playerID: ${UTF8ToString(playerId)} with value of ${value}`);


    } else {
      console.error('The player state object does not have a "setState" method.');
      return null;
    }
  },

  SetPlayerStateStringById: function (playerId, key, value) {
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
    if (typeof players !== 'object' || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error('Player with ID', UTF8ToString(playerId), 'not found.');
      return null;
    }

    // Assuming that the player state object has a "getProfile" method
    if (typeof playerState.setState === 'function') {
      playerState.setState(UTF8ToString(key), UTF8ToString(value));
      console.log(`setting state: ${UTF8ToString(key)} for playerID: ${UTF8ToString(playerId)} with value of ${UTF8ToString(value)}`);

    } else {
      console.error('The player state object does not have a "setState" method.');
      return null;
    }
  },


  GetPlayerStateIntById: function (playerId, key) {
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
    if (typeof players !== 'object' || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    // get the state of that player
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error('Player with ID', UTF8ToString(playerId), 'not found.');
      return null;
    }

    if (typeof playerState.getState === 'function') {
      var stateVal = playerState.getState(UTF8ToString(key));
      console.log(`Getting state: ${UTF8ToString(key)} for PlayerId: ${UTF8ToString(playerId)} : `, stateVal);
      return stateVal;

    } else {
      console.error('The player state object does not have a "getState" method.');
      return null;
    }
  },

  GetPlayerStateFloatById: function (playerId, key) {
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
    if (typeof players !== 'object' || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }


    // get the state of that player
    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error('Player with ID', UTF8ToString(playerId), 'not found.');
      return null;
    }


    if (typeof playerState.getState === 'function') {
      var stateVal = playerState.getState(UTF8ToString(key));
      console.log(`Getting state: ${UTF8ToString(key)} for player id: ${UTF8ToString(playerId)} : `, stateVal);
      return stateVal;
    } else {
      console.error('The player state object does not have a "getState" method.');
      return null;
    }
  },

  GetPlayerStateStringById: function (playerId, key) {
    const players = window._multiplayer.getPlayers();

    // Check if players is an object
    if (typeof players !== 'object' || players === null) {
      console.error('The "players" variable is not an object:', players);
      return null;
    }

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error('Player with ID', UTF8ToString(playerId), 'not found.');
      return null;
    }


    if (typeof playerState.getState === 'function') {
      var stateVal = playerState.getState(UTF8ToString(key));
      console.log(`getting this state: ${UTF8ToString(key)} for player id: ${UTF8ToString(playerId)} : `, stateVal);

      var bufferSize = lengthBytesUTF8(stateVal) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(stateVal, buffer, bufferSize);
      return buffer;

    }
    else {
      console.error('The player state object does not have a "getState" method.');
      return null;
    }
  },


  SETFloat: function (abc) {
    Playroom.setState("abc", abc)
    console.log("setting abc: ", abc)
  },

  GETFloat: function () {
    var abc = Playroom.getState("abc")
    console.log("Getting abc: ", abc);
    return abc;
  }

});
