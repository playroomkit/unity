using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    [Serializable]
    public class PlayerEntitlement<TMetadata>
    {
        public string id;
        public string paymentId;
        public string playerId;
        public string skuId;
        public bool free;
        public bool active;
        public bool deleted;
        public DateTime createdAt;
        public DateTime updatedAt;
        public DateTime? startDate;
        public DateTime? endDate;
        public string type;
        public string key;
        public TMetadata metadata; // create your own class for metadata based on your needs.

        private static PlayerEntitlement<TMetadata> FromJSONNode(JSONNode node, Func<string, TMetadata> metadataParser)
        {
            var rawMeta = node["metadata"]?.ToString() ?? "{}";

            PlayerEntitlement<TMetadata> data = new()
            {
                id = node["id"]?.Value ?? string.Empty,
                paymentId = node["paymentId"]?.Value ?? string.Empty,
                playerId = node["playerId"]?.Value ?? string.Empty,
                skuId = node["skuId"]?.Value ?? string.Empty,
                startDate = DateTime.TryParse(node["startDate"]?.Value, out var sAt) ? sAt : null,
                endDate = DateTime.TryParse(node["endDate"]?.Value, out var eAt) ? eAt : null,
                free = node["free"] != null && node["free"].AsBool,
                type = node["type"]?.Value ?? string.Empty,
                key = node["key"]?.Value ?? string.Empty,
                active = node["active"] != null && node["active"].AsBool,
                deleted = node["deleted"] != null && node["deleted"].AsBool,
                createdAt = DateTime.TryParse(node["createdAt"]?.Value, out var cAt) ? cAt : DateTime.MinValue,
                updatedAt = DateTime.TryParse(node["updatedAt"]?.Value, out var uAt) ? uAt : DateTime.MinValue,
                metadata = metadataParser(rawMeta)
            };
            return data;
        }

        public static List<PlayerEntitlement<TMetadata>> FromJSON(string jsonString, Func<string, TMetadata> metadataParser)
        {
            List<PlayerEntitlement<TMetadata>> entitlements = new();
            JSONNode root = JSON.Parse(jsonString);

            if (!root.IsArray)
                Debug.LogWarning("Expected an array");

            foreach (JSONNode item in root.AsArray)
            {
                var data = FromJSONNode(item, metadataParser);
                entitlements.Add(data);
            }

            return entitlements;
        }
    }
}