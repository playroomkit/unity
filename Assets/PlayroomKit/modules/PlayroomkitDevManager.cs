using UBB;
using UnityEngine;

namespace Playroom
{
    public class PlayroomkitDevManager : MonoBehaviour
    {
        [SerializeField] private PlayroomKit.MockModeSelector mockModeSelector = PlayroomKit.CurrentMockMode;

        [Tooltip("The GameObject which initializes multiplayer")] [SerializeField]
        private GameObject insertCoinCaller;

#if UNITY_EDITOR
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

        // Called 
        private void GetPlayerID(string playerId)
        {
            PlayroomKit.MockOnPlayerJoinWrapper(playerId);
        }
#endif
    }
}