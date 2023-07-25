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

      console.log('All CDNs have been loaded.');

      // example usecase
      Playroom.insertCoin();

    }).catch((error) => {
      console.error('Error loading CDNs:', error);
    });
  },

  // for testing
  Jump: function (str) {
    console.log(UTF8ToString(str));
  },

  // Function to call insertCoin from Unity
  InsertCoin: function () {
    Playroom.insertCoin();
  },


});
