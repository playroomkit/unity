using UBB;
using UnityEngine;

namespace Playroom
{
    public class PlayroomkitDevManager : MonoBehaviour
    {
        [SerializeField] private PlayroomKit.MockModeSelector mockMode = PlayroomKit.CurrentMockMode;

        [Tooltip(
            "InsertCoin() must be called in order to connect PlayroomKit server.\n\nChoose the gameObject (with the script) which calls InsertCoin.\n\nRead More in the docs")]
        [SerializeField]
        private GameObject insertCoinCaller;

        private static PlayroomkitDevManager Instance { get; set; }

#if UNITY_EDITOR
        private void Awake()
        {
            UpdateMockMode();

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (PlayroomKit.CurrentMockMode == PlayroomKit.MockModeSelector.BrowserBridge)
                UnityBrowserBridge.Instance.StartUBB();
        }

        private void OnValidate()
        {
            UpdateMockMode();
        }

        private void UpdateMockMode()
        {
            PlayroomKit.CurrentMockMode = mockMode;
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