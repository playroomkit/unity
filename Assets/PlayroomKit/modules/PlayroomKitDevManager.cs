using UBB;
using UnityEngine;

namespace Playroom
{
#if UNITY_EDITOR
    public class PlayroomKitDevManager : MonoBehaviour
    {
        [SerializeField] private PlayroomKit.MockModeSelector mockModeSelector = PlayroomKit.CurrentMockMode;

        [Tooltip("The GameObject which initializes multiplayer")] [SerializeField]
        private GameObject insertCoinCaller;


        private void Awake()
        {
            UpdateMockMode();
        }

        private void Start()
        {
            if (PlayroomKit.CurrentMockMode == PlayroomKit.MockModeSelector.BrowserBridgeMode)
            {
                UnityBrowserBridge.Instance.StartUBB();
            }
        }

        private void OnValidate()
        {
            UpdateMockMode();
        }

        private void UpdateMockMode()
        {
            PlayroomKit.CurrentMockMode = mockModeSelector;
            PlayroomKit.RegisterGameObject("InsertCoin", insertCoinCaller);
            PlayroomKit.RegisterGameObject("PlayerJoin", gameObject);
        }

        private void GetPlayerID(string playerId)
        {
            Debug.Log(playerId);
            PlayroomKit.MockOnPlayerJoinWrapper(playerId);
        }
    }
#endif
}