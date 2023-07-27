using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using System;
using SimpleJSON;



public class PlayroomKit : MonoBehaviour
{

    [DllImport("__Internal")]
    public static extern void GETFloat(float value);

    [DllImport("__Internal")]
    public static extern void LoadPlayroom();

    [DllImport("__Internal")]
    public static extern void InsertCoin(Action callback);

    [MonoPInvokeCallback(typeof(Action))]
    public static void CallBackInsertCoin()
    {
        //This fires from javascript
        Debug.Log("Insert Coin Callback Fired from Javascript defined in Unity");
    }


    [DllImport("__Internal")]
    public static extern bool IsHost();

    [DllImport("__Internal")]
    public static extern void SetStateString(string key, string value);

    [DllImport("__Internal")]
    public static extern void SetState(string key, int value);

    [DllImport("__Internal")]
    public static extern void SetState(string key, float value);

    [DllImport("__Internal")]
    public static extern void SetState(string key, bool value);

    [DllImport("__Internal")]
    public static extern string GetStateString(string key);

    [DllImport("__Internal")]
    public static extern int GetStateInt(string key);

    [DllImport("__Internal")]
    public static extern float GetStateFloat(string key);

    [DllImport("__Internal")]
    private static extern void SetStateDictionary(string key, string jsonValues);


    private static void SetStateHelper<T>(string key, Dictionary<string, T> values)
    {
        JSONObject jsonObject = new JSONObject();

        // Add key-value pairs to the JSON object
        foreach (var kvp in values)
        {
            // Convert the value to double before adding to JSONNode
            double value = Convert.ToDouble(kvp.Value);
            jsonObject.Add(kvp.Key, value);
        }

        // Serialize the JSON object to a string
        string jsonString = jsonObject.ToString();

        // Output the JSON string
        Debug.Log("Serialized JSON: " + jsonString);
        SetStateDictionary(key, jsonString);
    }

    public static void SetState(string key, Dictionary<string, int> values)
    {
        SetStateHelper(key, values);
    }

    public static void SetState(string key, Dictionary<string, float> values)
    {
        SetStateHelper(key, values);
    }

    public static void SetState(string key, Dictionary<string, bool> values)
    {
        SetStateHelper(key, values);
    }

    public static void SetState(string key, Dictionary<string, string> values)
    {
        SetStateHelper(key, values);
    }






    // public static void SetState(string key, Dictionary<string, int> values)
    // {

    //     JSONNode jsonNode = new JSONObject();

    //     // Add key-value pairs to the JSON object
    //     foreach (var kvp in values)
    //     {
    //         jsonNode[kvp.Key] = new JSONNumber(kvp.Value);
    //     }

    //     // Serialize the JSON object to a string
    //     string jsonString = jsonNode.ToString();

    //     // Output the JSON string
    //     Debug.Log("Serialized JSON: " + jsonString);
    //     SetStateDictionary(key, jsonString);
    // }







    public static bool GetStateBool(string key)
    {
        if (GetStateInt(key) == 1)
        {
            return true;
        }
        else if (GetStateInt(key) == 0)
        {
            return false;
        }
        else
        {
            Debug.LogError("GetStateBool: " + key + " is not a bool");
            return false;
        }

    }



    // it checks if the game is running in the browser or in the editor
    public static bool IsRunningInBrowser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}
