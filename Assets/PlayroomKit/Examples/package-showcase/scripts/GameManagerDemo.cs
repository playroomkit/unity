using System;
using System.Collections.Generic;
using AOT;
using OpenQA.Selenium.DevTools.V94.Debugger;
using Playroom;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEditor;

public class GameManagerDemo : MonoBehaviour
{
    private static readonly List<PlayroomKit.Player> players = new();
    private static readonly List<GameObject> playerGameObjects = new();

    private static readonly Dictionary<string, GameObject> PlayerDict = new();

    [SerializeField] private static bool playerJoined;
    [SerializeField] private string roomCode;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private TMP_Dropdown getDropDown;
    [SerializeField] private TMP_Dropdown colorDropDown;
    
    [SerializeField] private int score = 0;


    public void InsertCoin()
    {
        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions
        {
            maxPlayersPerRoom = 3,
            matchmaking = false,
            gameId = "<Insert GameID (from dev.joinplayroom.com)>",
            discord = true,
            roomCode = roomCode,
        }, () => { PlayroomKit.OnPlayerJoin(AddPlayer); }, () => { Debug.Log("OnDisconnect callback"); });
    }
    
    
    public void AddPlayer(PlayroomKit.Player player)
    {
        var playerObj = Instantiate(playerPrefab,
            new Vector3(Random.Range(-5, 5), 2f, Random.Range(-5, 5)), Quaternion.identity);

        player.SetState("color", player.GetProfile().color);

        PlayerDict.Add(player.id, playerObj);
        players.Add(player);
        playerGameObjects.Add(playerObj);


        for (var i = 0; i < players.Count; i++) Debug.Log($"player at index {i} is {players[i].GetProfile().name}");


        playerJoined = true;
        player.OnQuit(RemovePlayer);
    }


    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void RemovePlayer(string playerID)
    {
        if (PlayerDict.TryGetValue(playerID, out var player))
        {
            PlayerDict.Remove(playerID);
            playerGameObjects.Remove(player);
            Destroy(player);
        }
        else
        {
            Debug.LogWarning("player not in dict");
        }
    }
    
    public void SetStatePosition()
    {
        if (playerJoined)
        {
            var myPlayer = PlayroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);

            playerGameObjects[index].GetComponent<PlayerController>().LookAround();
            players[index].SetState("angle", playerGameObjects[index].GetComponent<Transform>().rotation);

            playerGameObjects[index].GetComponent<PlayerController>().Move();
            players[index].SetState("move", playerGameObjects[index].GetComponent<Transform>().position);
            
        }
    }

    public void GetStatePosition()
    {
        if (playerJoined)
        {
            for (var i = 0; i < players.Count; i++)
                if (players[i] != null)
                {
                    var pos = players[i].GetState<Vector3>("move");
                    var rotate = players[i].GetState<Quaternion>("angle");
            
                    if (playerGameObjects[i] != null)
                    {
                        playerGameObjects[i].GetComponent<Transform>().SetPositionAndRotation(pos, rotate);
                    }
                }
        }
    }

    public void SetStateColor(Color color)
    {
        if (playerJoined)
        {
            var myPlayer = PlayroomKit.MyPlayer();
            myPlayer.SetState("color",color);
        }
    }

    public void GetPlayerProfile()
    {
        var profile = PlayroomKit.MyPlayer().GetProfile();
        var name = profile.name;
        var color = profile.color;
        var photo = profile.photo;
        Debug.Log($"name: {name}, color: {color}, photo: {photo}");
    }

    public void GetStateColor()
    {
        if (playerJoined)
        {
            for (var i = 0; i < players.Count; i++)
                if (players[i] != null)
                {
                    var color = players[i].GetState<Color>("color");
            
                    if (playerGameObjects[i] != null)
                    {
                        playerGameObjects[i].GetComponent<Renderer>().material.color = color;
                    }
                }
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
            default:
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
                SetStateColor(color:color);
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    public void HandleValueChange(int index)
    {
        colorDropDown.gameObject.SetActive(index == 1);
    }

    public void RegisterRpcShoot()
    {
        PlayroomKit.RpcRegister("ShootLaser", HandleScoreUpdate, "You shot a bullet!");
        Debug.Log($"Shoot function registered");
    }
    
    void HandleScoreUpdate(string data, string caller)
    {
        var player = PlayroomKit.GetPlayer(caller);
        Debug.Log($"Caller: {caller}, Player Name: {player?.GetProfile().name}, Data: {data}");

        if (PlayerDict.TryGetValue(caller, out GameObject playerObj))
        {
            var playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.scoreText.text = $"{data}";
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

    }
    
    
    public void ShootLaser()
    {
            var myPlayer = PlayroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);
            score = playerGameObjects[index].GetComponent<RaycastGun>().ShootLaser(score);
            PlayroomKit.RpcCall("ShootLaser", score, PlayroomKit.RpcMode.OTHERS,  () =>
            {
                Debug.Log("Shooting bullet");
            });
    }

    public void GetRoomCode()
    {
       var  roomcode = PlayroomKit.GetRoomCode();
       Debug.Log($"Room code: {roomcode}");
    }
    
    // Update is called once per frame
    void Update()
    {
        if (playerJoined)
        {
            var myPlayer = PlayroomKit.MyPlayer();
            var index = players.IndexOf(myPlayer);
            
            //ShootLaser(index);

            playerGameObjects[index].GetComponent<PlayerController>().LookAround();
            playerGameObjects[index].GetComponent<PlayerController>().Move();
        }
    }
    
}
