using System;
using System.Collections.Generic;
using SimpleJSON;

namespace Discord
{
    [Serializable]
    public class DiscordEntitlement
    {
        // Required
        public string Id;
        public string SkuId;
        public string ApplicationId;
        public string UserId;
        public int GiftCodeFlags;
        public string Type;             // always a string (coerced from number if needed)

        // Optionals
        public string? GifterUserId;
        public List<string>? Branches;
        public string? StartsAt;   // ISO-8601 as string
        public string? EndsAt;
        public string? ParentId;
        public bool? Consumed;
        public bool? Deleted;
        public string? GiftCodeBatchId;

        private static DiscordEntitlement FromJSONNode(JSONNode node)
        {
            // Required fields
            if (!node.HasKey("type"))
                throw new FormatException("Required field 'type' is missing.");

            var e = new DiscordEntitlement
            {
                Id = node["id"].Value,
                SkuId = node["sku_id"].Value,
                ApplicationId = node["application_id"].Value,
                UserId = node["user_id"].Value,
                GiftCodeFlags = node["gift_code_flags"].AsInt,
                Type = node["type"].IsNumber ? node["type"].AsInt.ToString() : node["type"].Value
            };

            // Optionals
            if (node.HasKey("gifter_user_id"))
                e.GifterUserId = node["gifter_user_id"].Value;

            if (node.HasKey("branches"))
            {
                e.Branches = new List<string>();
                foreach (var b in node["branches"].AsArray)
                    e.Branches.Add(b.Value);
            }

            if (node.HasKey("starts_at"))
                e.StartsAt = node["starts_at"].Value;

            if (node.HasKey("ends_at"))
                e.EndsAt = node["ends_at"].Value;

            if (node.HasKey("parent_id"))
                e.ParentId = node["parent_id"].Value;

            if (node.HasKey("consumed"))
                e.Consumed = node["consumed"].AsBool;

            if (node.HasKey("deleted"))
                e.Deleted = node["deleted"].AsBool;

            if (node.HasKey("gift_code_batch_id"))
                e.GiftCodeBatchId = node["gift_code_batch_id"].Value;

            return e;
        }

        /// <summary>
        /// Parse a JSON string into a list of DiscordEntitlement instances.
        /// </summary>
        public static List<DiscordEntitlement> FromJSON(string jsonString)
        {
            var list = new List<DiscordEntitlement>();
            var root = JSON.Parse(jsonString);

            if (root == null)
                throw new FormatException("Invalid JSON: root is null.");

            if (root.HasKey("entitlements") && root["entitlements"].IsArray)
            {
                foreach (var item in root["entitlements"].AsArray)
                    list.Add(FromJSONNode(item));
            }
            else if (root.IsArray)
            {
                foreach (var item in root.AsArray)
                    list.Add(FromJSONNode(item));
            }
            else
            {
                list.Add(FromJSONNode(root));
            }

            return list;
        }
    }
}