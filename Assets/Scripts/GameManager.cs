using System;
using Playroom;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit _kit;
    
    //enum test example 
    public enum  Gun
    {
        shoot,
        reload
    }

    public Gun Guns;
    
        

    private void Awake()
    {
        _kit = new PlayroomKit();
    }

    private void Start()
    {
        _kit.InsertCoin(new InitOptions()
        {
            turnBased = true,
            maxPlayersPerRoom = 2,
        }, OnLaunchCallBack);
    }

    private void OnLaunchCallBack()
    {
        _kit.OnPlayerJoin(CreatePlayer);
    }

    private void CreatePlayer(PlayroomKit.Player player)
    {
        Debug.Log($"{player.id} joined the room!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Guns = (Gun)(((int)Guns + 1) % System.Enum.GetValues(typeof(Gun)).Length);
            Debug.Log(Guns);
        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log($"Saving my turn data...");
        }
    }
}