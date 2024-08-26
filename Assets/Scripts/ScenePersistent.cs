using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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