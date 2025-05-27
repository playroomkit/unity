using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    [Serializable]
    public class SKU<TMetadata>
    {
        public string id;
        public string name;
        public string description;
        public string type;
        public string image;
        public string key;
        public bool active;
        public bool deleted;
        public DateTime createdAt;
        public DateTime updatedAt;
        public TMetadata metadata; // create your own class for metadata based on your needs.
        public string price;
        public string productId;

        private static SKU<TMetadata> FromJSONNode(JSONNode node, Func<string, TMetadata> metadataParser)
        {
            var rawMeta = node["metadata"]?.ToString() ?? "{}";
            

            SKU<TMetadata> data = new()
            {
                id = node["id"]?.Value ?? string.Empty,
                name = node["name"]?.Value ?? string.Empty,
                description = node["description"]?.Value ?? string.Empty,
                type = node["type"]?.Value ?? string.Empty,
                image = node["image"]?.Value,
                key = node["key"]?.Value ?? string.Empty,
                active = node["active"] != null && node["active"].AsBool,
                deleted = node["deleted"] != null && node["deleted"].AsBool,
                price = node["price"]?.Value ?? string.Empty,
                productId = node["productId"]?.Value ?? string.Empty,
                createdAt = DateTime.TryParse(node["createdAt"]?.Value, out var cAt) ? cAt : DateTime.MinValue,
                updatedAt = DateTime.TryParse(node["updatedAt"]?.Value, out var uAt) ? uAt : DateTime.MinValue,
                metadata = metadataParser(rawMeta)
            };
            return data;
        }

        public static List<SKU<TMetadata>> FromJSON(string jsonString, Func<string, TMetadata> metadataParser)
        {
            List<SKU<TMetadata>> skus = new();
            JSONNode root = JSON.Parse(jsonString);

            if (!root.IsArray)
                Debug.LogWarning("Expected an array of SKUs");

            foreach (JSONNode item in root.AsArray)
            {
                SKU<TMetadata> data = FromJSONNode(item, metadataParser);
                skus.Add(data);
            }

            return skus;
        }
    }
}