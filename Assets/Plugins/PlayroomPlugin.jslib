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

    var returnVal = Playroom.getState(UTF8ToString(key));
    console.log("float coming from unity: ", returnVal)

    return Playroom.getState(UTF8ToString(key));
  },

  GetStateString: function (key) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }
    console.log("key", UTF8ToString(key), "value", Playroom.getState(UTF8ToString(key)))

    // retrun value to unity
    var returnStr = Playroom.getState(UTF8ToString(key));

    console.log("returnStr: ", returnStr)

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

    // Parse the JSON string back into a JavaScript object
    console.log("jsonValues from unity: ", UTF8ToString(jsonValues))

    // var values = JSON.parse(jsonValues);
    // console.log("key", UTF8ToString(key), "values", values);
    // Playroom.setState(UTF8ToString(key), values);
  },



  IsHost: function () {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }

    // retrun value to unity
    return Playroom.isHost();
  },

  OnPlayerJoin: function (functionPtr) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }


    Playroom.onPlayerJoin((player) => {
      // Call the C# callback function with the player.id as a string parameter

      var id = player.id;
      console.log("id: ", id)
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

    console.log("Players: ", players);

    const playerState = players[UTF8ToString(playerId)];

    if (!playerState) {
      console.error('Player with ID', UTF8ToString(playerId), 'not found.');
      return null;
    }

    console.log("PlayerState", playerState);

    // Assuming that the player state object has a "getProfile" method
    if (typeof playerState.getProfile === 'function') {
      const profile = playerState.getProfile();
      console.log("Log profile: ", profile);

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


  getStateByPlayerStateId: function (playerStateId, key, value) {
    window._multiplayer.getPlayers().find(playerState => playerState.id === playerStateId).getState(key, value);
  },

  setStateByPlayerStateId: function (playerStateId, key, value) {
    window._multiplayer.getPlayers().find(playerState => playerState.id === playerStateId).setState(key, value);
  },

  // send float value to unity 
  GETFloat: function (abc) {

    console.log("abc: ", abc)

  }

});
