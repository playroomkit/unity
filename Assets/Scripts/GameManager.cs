using System;
using Playroom;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit _kit;
    
    //enum test example 
    public enum  Gun
    {
        idle,
        shooting,
        reload
    }

    public Gun gunsAction;
    
        

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
           
            Gun nextGunState = (Gun)(((int)gunsAction + 1) % System.Enum.GetValues(typeof(Gun)).Length);

            Debug.Log($"Current Gun State: {gunsAction}, Next Gun State: {nextGunState}");

            gunsAction = nextGunState;
            
            _kit.SetState("gunState", nextGunState);
            Debug.Log($"State set to: {nextGunState}");

            Gun retrievedState = _kit.GetState<Gun>("gunState");
            Debug.Log($"Retrieved Gun State: {retrievedState}");
        }



        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log($"Saving my turn data...");
        }
    }
}