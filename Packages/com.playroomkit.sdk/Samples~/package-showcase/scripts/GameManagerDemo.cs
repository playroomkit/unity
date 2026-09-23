using System;
using System.Collections.Generic;
using AOT;
using Playroom;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManagerDemo : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();
    private static readonly Dictionary<string, GameObject> PlayerDict = new();

    [SerializeField]
    private static bool playerJoined;
    [SerializeField]
    private string roomCode;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private int score;

    [Header("UI")]
    [SerializeField]
    private TMP_Dropdown getDropDown;
    [SerializeField]
    private TMP_Dropdown colorDropDown;
    [SerializeField]
    private TextMeshProUGUI logsText;

    private PlayroomKit _playroomKit = new();

    [Serializable]
    private class TempData
    {
        public Vector3 pos;
        public string message;
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerJoined)
        {
            var myPlayer = _playroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            //ShootLaser(index);
            playerGameObjects[index].GetComponent<PlayerController>().LookAround();
            playerGameObjects[index].GetComponent<PlayerController>().Move();
        }
    }

    public void InsertCoin()
    {
        _playroomKit.InsertCoin(new InitOptions
        {
            skipLobby = true,
            maxPlayersPerRoom = 3,
            matchmaking = false,
            roomCode = roomCode
        }, () =>
        {
            Debug.Log("Game Launched!");
            logsText.text = "Coin Inserted Game Launched";
        }, () =>
        {
            Debug.Log("OnDisconnect callback");
            logsText.text = "OnDisconnect Invoked";
        });
    }

    public void OnPlayerJoin()
    {
        _playroomKit.OnPlayerJoin(AddPlayer);
        logsText.text = "OnPlayerJoin called, invoking Add Player";
    }

    private void AddPlayer(PlayroomKit.Player player)
    {
        var playerObj = Instantiate(playerPrefab,
            new Vector3(Random.Range(-5, 5), 2f, Random.Range(-5, 5)), Quaternion.identity);

        player.SetState("color", player.GetProfile().color);

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);


        for (var i = 0; i < players.Count; i++) Debug.Log($"player at index {i} is {players[i].GetProfile().name}");

        playerJoined = true;


        logsText.text = "Adding Player";

        player.OnQuit(RemovePlayer);
    }


    private void RemovePlayer(string playerID)
    {
        if (PlayerDict.TryGetValue(playerID, out var player))
        {
            PlayerDict.Remove(playerID);
            playerGameObjects.Remove(player);
            logsText.text = "Player Removed";
            Destroy(player);
        }
        else
        {
            Debug.LogWarning("player not in dict");
            logsText.text = "OnPlayerJoin called, invoking Add Player";
        }
    }

    private void SetStatePosition()
    {
        if (playerJoined)
        {
            var myPlayer = _playroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            playerGameObjects[index].GetComponent<PlayerController>().LookAround();
            players[index].SetState("angle", playerGameObjects[index].GetComponent<Transform>().rotation);

            playerGameObjects[index].GetComponent<PlayerController>().Move();
            players[index].SetState("move", playerGameObjects[index].GetComponent<Transform>().position);

            logsText.text =
                $"Setting Position of Player at: {playerGameObjects[index].GetComponent<Transform>().position}";
        }
    }

    private void GetStatePosition()
    {
        if (playerJoined)
            for (var i = 0; i < players.Count; i++)
                if (players[i] != null)
                {
                    var pos = players[i].GetState<Vector3>("move");
                    logsText.text = $"Getting and Player Pos: {pos}";


                    var rotate = players[i].GetState<Quaternion>("angle");

                    if (playerGameObjects[i] != null)
                        playerGameObjects[i].GetComponent<Transform>().SetPositionAndRotation(pos, rotate);
                }
    }

    private void SetStateColor(Color color)
    {
        if (playerJoined)
        {
            var myPlayer = _playroomKit.MyPlayer();

            var data = new TempData()
            {
                message = "Sending Pos",
                pos = new Vector3(6f, 1f, 4f)
            };

            Debug.LogWarning($"Setting data : {data}");

            string d = JsonUtility.ToJson(data);
            myPlayer.SetState("data", d);

            myPlayer.SetState("color", color);
            logsText.text = $"setting color to {color}";
        }
    }

    public void GetPlayerProfile()
    {
        var profile = _playroomKit.MyPlayer().GetProfile();
        var name = profile.name;
        var color = profile.color;
        var photo = profile.photo;
        Debug.Log($"name: {name}, color: {color}, photo: {photo}");
        logsText.text = $"Getting Player Profile\n\nName: {name}\nColor: {color}\nPhotoURL: {photo}\n";
    }

    private void GetStateColor()
    {
        if (playerJoined)
            for (var i = 0; i < players.Count; i++)
                if (players[i] != null)
                {
                    Debug.LogWarning($"Getting data ${players[i].GetState<string>("data")}");

                    var color = players[i].GetState<Color>("color");

                    if (playerGameObjects[i] != null)
                        playerGameObjects[i].GetComponent<Renderer>().material.color = color;
                }
    }


    public void GetState()
    {
        var option = getDropDown.value;
        switch (option)
        {
            case 0:
                GetStatePosition();
                break;
            case 1:
                GetStateColor();
                break;
            case 2:
                break;
        }
    }

    public void SetState()
    {
        var option = getDropDown.value;
        switch (option)
        {
            case 0:
                SetStatePosition();
                break;
            case 1:
                var colorOption = colorDropDown.value;
                var color = Color.white;
                switch (colorOption)
                {
                    case 0:
                        color = Color.red;
                        break;
                    case 1:
                        color = Color.blue;
                        break;
                    case 2:
                        color = Color.green;
                        break;
                    case 3:
                        color = Color.yellow;
                        break;
                    default:
                        color = Color.cyan;
                        break;
                }

                SetStateColor(color);
                break;
            case 2:
                break;
        }
    }

    public void HandleValueChange(int index)
    {
        colorDropDown.gameObject.SetActive(index == 1);
    }

    public void RegisterRpcShoot()
    {
        _playroomKit.RpcRegister("hostCall", hostCall);
        _playroomKit.RpcRegister("clientCall", clientCall);

        Debug.Log("Shoot function registered");
        logsText.text = "ShootLaser RPC registered";
    }

    private void HandleScoreUpdate(string data, string caller)
    {
        Debug.LogWarning("Handle Score Called");

        if (PlayerDict.TryGetValue(caller, out var playerObj))
        {
            var playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.scoreText.text = $"{data}";
                logsText.text = $"Data from RPC: {data}";
            }
            else
            {
                Debug.LogError($"PlayerController not found on GameObject for caller: {caller}");
            }
        }
        else
        {
            Debug.LogError($"No GameObject found for caller: {caller}");
        }

        _playroomKit.RpcCall("ChainCall", "", PlayroomKit.RpcMode.ALL);
    }

    private void hostCall(string data, string senderID)
    {
        Debug.LogWarning("HOST CALL");
        _playroomKit.RpcCall("clientCall", "", PlayroomKit.RpcMode.ALL);
    }

    private void clientCall(string data, string senderID)
    {
        Debug.LogWarning("CLIENT CALL");
    }


    public void ShootLaser()
    {
        var myPlayer = _playroomKit.MyPlayer();
        var index = players.IndexOf(myPlayer);
        score = playerGameObjects[index].GetComponent<Laser>().ShootLaser(score);

        // _playroomKit.RpcCall("ShootLaser", score, PlayroomKit.RpcMode.ALL,
        //     () =>
        //     {
        //         Debug.Log("ShootLaser RPC Called");
        //         logsText.text = "ShootLaser RPC Called";
        //     });

        _playroomKit.RpcCall("hostCall", "", PlayroomKit.RpcMode.HOST);
    }

    public void GetRoomCode()
    {
        var roomcode = _playroomKit.GetRoomCode();
        Debug.Log($"Room code: {roomcode}");
        logsText.text = $"Current RoomCode: {roomcode}";
    }

    public void IsHost()
    {
        bool isHost = _playroomKit.IsHost();
        Debug.Log("isHost: " + isHost);
        logsText.text = $"{_playroomKit.MyPlayer().GetProfile().name} is host?: {isHost}";
    }


    /// <summary>
    /// First Click the WaitForState button, then set the color using the dropdown and pressing the SetState button
    /// </summary>
    public void WaitForState()
    {
        logsText.text = "Waiting for color to be set";
        _playroomKit.MyPlayer().WaitForState("color",
            data =>
            {
                Debug.Log($"data from WaitForPlayer state, color: {data}");

                logsText.text += $"\nData from WaitForPlayer state, color: {data}";
            });
    }

    public void ResetPlayerStates()
    {
        _playroomKit.ResetPlayersStates(null, () =>
        {
            logsText.text =
                $"All Player States were Reset\n e.g: Color: {_playroomKit.MyPlayer().GetState<Color>("color")}";

            var player = _playroomKit.MyPlayer();
            var color = player.GetState<Color>("color");
            var pos = player.GetState<Vector3>("pos");

            if (PlayerDict.TryGetValue(player.id, out var Player))
            {
                Player.GetComponent<Renderer>().material.color = color;
                Player.GetComponent<Transform>().position = pos;
            }
        });
    }

    public void GlobalSetState()
    {
        _playroomKit.SetState("winner", "ChickenGamer");
    }

    public void GlobalGetState()
    {
        logsText.text = $"getting global (state): Winner is : {_playroomKit.GetState<string>("winner")}";
    }
}