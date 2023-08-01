using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using System;
using SimpleJSON;


public class PlayroomKit
{

    public class Player
    {
        public string playerId;


        public void SetState(string key, int value)
        {
            Debug.Log("player id in setstate unity: " + playerId);
            SetPlayerStateByPlayerId(playerId, key, value);
        }

        [DllImport("__Internal")]
        private static extern string SetPlayerStateByPlayerId(string playerID, string key, int value);

        [DllImport("__Internal")]
        private static extern string SetPlayerStateByPlayerId(string playerID, string key, float value);

        [DllImport("__Internal")]
        private static extern string SetPlayerStateByPlayerId(string playerID, string key, bool value);

        [DllImport("__Internal")]
        private static extern string SetPlayerStateStringById(string playerID, string key, string value);


        [DllImport("__Internal")]
        public static extern string GetProfileByPlayerId(string playerID);

        [DllImport("__Internal")]
        public static extern int GetPlayerStateIntById(string playerID, string key);

        [DllImport("__Internal")]
        public static extern float GetPlayerStateFloatById(string playerID, string key);

        [DllImport("__Internal")]
        public static extern string GetPlayerStateStringById(string playerID, string key);

        public static bool GetPlayerStateBoolById(string playerID, string key)
        {
            if (GetPlayerStateIntById(playerID, key) == 1)
            {
                return true;
            }
            else if (GetPlayerStateIntById(playerID, key) == 0)
            {
                return false;
            }
            else
            {
                Debug.LogError("GetPlayerStateByPlayerId: " + key + " is not a bool");
                return false;
            }

        }

    }

    private static Player currentPlayer = new Player();

    public static string playerId;

    [DllImport("__Internal")]
    public static extern float GETFloat();

    [DllImport("__Internal")]
    public static extern void SETFloat(float value);


    [DllImport("__Internal")]
    public static extern void InsertCoin(Action callback);


    [DllImport("__Internal")]
    public static extern bool IsHost();

    [DllImport("__Internal")]
    private static extern void SetStateString(string key, string value);

    [DllImport("__Internal")]
    public static extern void SetState(string key, int value);

    [DllImport("__Internal")]
    public static extern void SetState(string key, float value);

    [DllImport("__Internal")]
    public static extern void SetState(string key, bool value);

    public static void SetState(string key, string value)
    {
        SetStateString(key, value);
    }

    [DllImport("__Internal")]
    public static extern string GetStateString(string key);

    [DllImport("__Internal")]
    public static extern int GetStateInt(string key);

    [DllImport("__Internal")]
    public static extern float GetStateFloat(string key);

    [DllImport("__Internal")]
    private static extern void SetStateDictionary(string key, string jsonValues);


    [DllImport("__Internal")]
    private static extern void OnPlayerJoinInternal(Action<string> callback);

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void WrapperCallback(string id)
    {
        Debug.Log("id from jslib: " + id);
        currentPlayer.playerId = id;
        Debug.Log("ID in curentPlayer: " + currentPlayer.playerId);
    }


    public static void OnPlayerJoin(Action<Player> playerCallBack)
    {

        OnPlayerJoinInternal(WrapperCallback);
        Debug.Log("OnPlayerJoin: " + currentPlayer.playerId);

        // TODO: delay the callback to make sure the playerId is set before calling the callback
        // playerCallBack?.Invoke(currentPlayer);
    }


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
