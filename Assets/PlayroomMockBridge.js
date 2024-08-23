InsertCoin = async function (options, onLaunchCallBackName, gameObjectName) {
    function onLaunchCallBack() {
        unityInstance.SendMessage(gameObjectName, onLaunchCallBackName);
    }

    await Playroom.insertCoin(options, onLaunchCallBack);
};

OnPlayerJoin = function (gameObjectName) {
    Playroom.onPlayerJoin((player) => {
        console.log("Player joined: " + player.id);

        unityInstance.SendMessage(gameObjectName, "GetPlayerID", player.id);
    });
};


SetState = function (key, value, reliable) {
    reliable = !!reliable;
    
    console.log(key, value, reliable)
    Playroom.setState(key, value, reliable)
}

GetState = function (key) {
    console.log(Playroom.getState(key))
    return Playroom.getState(key)
}


// Refactor:
SetPlayerStateByPlayerId = function (playerId, key, value, reliable) {
    const players = window._multiplayer.getPlayers();

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

        // console.log(`Key: ${key}: Value: ${value} reliable: ${reliable} playerID: ${playerId}`)

        // value = JSON.stringify(value)

        playerState.setState(key, value, reliable);
    } else {
        console.error(
            'The player state object does not have a "setState" method.'
        );
        return null;
    }
}

GetPlayerStateByPlayerId = function (playerId, key) {
    const players = window._multiplayer.getPlayers();

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

            // console.log(stateVal)

            JSON.stringify(stateVal)

            return stateVal;
        } catch (error) {
            console.log("There was an error: " + error);
        }
    } else {
        console.error(
            'The player state object does not have a "getState" method.'
        );
        return null;
    }
};