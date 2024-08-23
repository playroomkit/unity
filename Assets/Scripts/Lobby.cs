using UnityEngine;
using Playroom;
using TMPro;
using UnityEngine.Serialization;

public class Lobby : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerID;

    [SerializeField] private PlayroomKit.MockModeSelector mockModeSelector;

    public void Initialize()
    {
        PlayroomKit.InsertCoin(new PlayroomKit.InitOptions()
        {
            maxPlayersPerRoom = 2,
            defaultPlayerStates = new()
            {
                { "score", 0 },
            },
        }, () => { PlayroomKit.OnPlayerJoin(AddPlayer); });
    }

    private void AddPlayer(PlayroomKit.Player player)
    {
        Debug.Log("Player ID: " + player.id);


        playerID.text += $"{player.id} joined the game!";
    }
}