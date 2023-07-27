mergeInto(LibraryManager.library, {

  // Function to load the required external JavaScript files including Playroom
  LoadPlayroom: function () {
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
      // Playroom.insertCoin();

    }).catch((error) => {
      console.error('Error loading CDNs:', error);
    });
  },


  // Function to call insertCoin from Unity
  InsertCoin: function (functionPtr) {
    if (!window.Playroom) {
      console.error('Playroom library is not loaded. Please make sure to call LoadPlayroom first.');
      return;
    }

    Playroom.insertCoin().then(() => {

      dynCall("v", functionPtr, []);

    }).catch((error) => {
      console.error('Error inserting coin:', error);
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
    console.log("jsonValues from unity", UTF8ToString(jsonValues))
    
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

  // send float value to unity 
  GETFloat: function (abc) {

    console.log("abc: ", abc)

  }

});
