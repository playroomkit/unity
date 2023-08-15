using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AOT;
using Playroom;


public class TestManager : MonoBehaviour
{
    
    public void SetStateTest()
    {
        PlayroomKit.SetState("test", 3.14f);

        
    }

    public void GetState()
    {
        Debug.Log("Getting Float: " + PlayroomKit.GetState<float>("test"));
    }

}
