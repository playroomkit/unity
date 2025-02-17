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
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log($"Challenge Id: {_kit.GetChallengeId()}");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log($"Saving my turn data...");
            _kit.SaveMyTurnData(_kit.Me().id);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            _kit.GetMyTurnData((data) =>
            {
                Debug.Log($"Getting my turn data: " + $"{data.id}, {data.player.GetProfile().name}, {data.data}");
            });
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _kit.GetAllTurns((allData) =>
            {
                for (var i = 0; i < allData.Count; i++)
                {
                    var data = allData[i];
                    Debug.Log($"at index ${i}: {data.id}, {data.player.GetProfile().name}, {data.data}");
                }
            });
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            _kit.ClearTurns(() => { Debug.Log("Cleared all turns data!"); });
        }
    }
}