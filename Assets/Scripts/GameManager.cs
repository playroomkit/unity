using Playroom;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit _kit;

    private void Awake()
    {
        _kit = new PlayroomKit();
    }

    private void Start()
    {
        _kit.InsertCoin(new InitOptions()
        {
            turnBased = new TurnBasedOptions()
            {
                challengeId = "123"
            },
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
        if (Input.GetKey(KeyCode.C))
        {
            Debug.Log($"Challenge Id: {_kit.GetChallengeId()}");
        }
    }
}