using UnityEngine;

namespace Playroom
{
#if UNITY_EDITOR
    public class PlayroomBrowserBridge : MonoBehaviour
    {
        [SerializeField] private PlayroomKit.MockModeSelector mockModeSelector = PlayroomKit.CurrentMockMode;


        // Helper method called from JavaScript side
        private void GetPlayerID(string playerId)
        {
            PlayroomKit.MockOnPlayerJoinWrapper(playerId);
        }
    }
#endif
}