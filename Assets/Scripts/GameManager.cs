using System;
using System.Collections.Generic;
using UnityEngine;
using Playroom;



public class GameManager : MonoBehaviour
{
    public static GameObject playerPrefab;



    private void Start()
    {
        PlayroomKit.InsertCoin(() =>
        {
            PlayroomKit.OnPlayerJoin(AddPlayer);
        });
    }

    public static void AddPlayer(PlayroomKit.Player player)
    {
        var newPlayer = Instantiate(Resources.Load("Player"), new Vector3(-4, 4, 0), Quaternion.identity);
    }
}