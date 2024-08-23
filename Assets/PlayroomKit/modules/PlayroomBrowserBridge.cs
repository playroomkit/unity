using UnityEngine;

namespace Playroom
{
#if UNITY_EDITOR
    public class PlayroomBrowserBridge : MonoBehaviour
    {
        [SerializeField] private PlayroomKit.MockModeSelector mockModeSelector = PlayroomKit.CurrentMockMode;

        // This method is called when the script instance is loaded
        private void Awake()
        {
            UpdateMockMode();
        }

        // This method is called whenever the value of mockModeSelector changes in the Inspector
        private void OnValidate()
        {
            UpdateMockMode();
        }

        // Update the CurrentMockMode to match the selected mock mode
        private void UpdateMockMode()
        {
            PlayroomKit.CurrentMockMode = mockModeSelector;
            Debug.Log($"Current Mock Mode set to: {PlayroomKit.CurrentMockMode}");
        }

        // Helper method called from JavaScript side
        private void GetPlayerID(string playerId)
        {
            PlayroomKit.MockOnPlayerJoinWrapper(playerId);
        }
    }
#endif
}