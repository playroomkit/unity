using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    [Serializable]
    public class ServerReward
    {
        public string serverId;
        public string id;
        public string title;
        public string description;
        public string image;
        public string message;
        public bool active;
        public string key;
        public string skuId;
        public string type;
        public bool status;

        private static ServerReward FromJSONNode(JSONNode node)
        {
            ServerReward reward = new ServerReward
            {
                serverId = node["server_id"]?.Value ?? string.Empty,
                id = node["id"]?.Value ?? string.Empty,
                title = node["title"]?.Value ?? string.Empty,
                description = node["description"]?.Value ?? string.Empty,
                image = node["image"]?.Value ?? string.Empty,
                message = node["message"]?.Value ?? string.Empty,
                active = node["active"] != null && node["active"].AsBool,
                key = node["key"]?.Value ?? string.Empty,
                skuId = node["sku_id"]?.Value ?? string.Empty,
                type = node["type"]?.Value ?? string.Empty,
                status = node["status"] != null && node["status"].AsBool
            };

            return reward;
        }

        public static List<ServerReward> FromJSON(string jsonString)
        {
            List<ServerReward> serverRewards = new();
            JSONNode root = JSON.Parse(jsonString);

            if (!root.IsArray)
                Debug.LogWarning("Expected an array");

            foreach (JSONNode item in root.AsArray)
            {
                var data = FromJSONNode(item);
                serverRewards.Add(data);
            }

            return serverRewards;
        }
    }
}