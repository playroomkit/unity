using Playroom;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayroomKit _kit;

    // Enum Test Example 
    public enum Gun
    {
        idle,
        shooting,
        reload
    }

    public enum Player
    {
        idle,
        walk,
    }

    public Gun gunsAction = Gun.shooting;
    public Player playersAction = Player.walk;

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
            _kit.SetState("gunState", gunsAction);
            _kit.MyPlayer().SetState("playerState", playersAction);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunsAction = Gun.reload;
            _kit.SetState("gunState", gunsAction);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Gun retrievedState = _kit.GetState<Gun>("gunState");
            Debug.LogWarning($"Retrieved Gun State: {retrievedState}");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Player retrievedState = _kit.MyPlayer().GetState<Player>("playerState");
            Debug.LogWarning($"Retrieved Player State: {retrievedState}");
        }
    }
}