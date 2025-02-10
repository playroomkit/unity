using UnityEngine;

namespace CI.PowerConsole.Core
{
    public class CanvasController : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}