using UnityEngine;

#if UNITY_EDITOR
public class ScenePersistent : MonoBehaviour
{
    private static ScenePersistent Instance { get; set; }

    void Awake()
    {
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
}
#endif