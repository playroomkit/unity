using System;
using System.Collections.Generic;
using SimpleJSON;

namespace Discord
{
    public enum DiscordSkuType
    {
        UNHANDLED = -1,
        APPLICATION = 1,
        DLC = 2,
        CONSUMABLE = 3,
        BUNDLE = 4,
        SUBSCRIPTION = 5
    }

    [Serializable]
    public class DiscordSkuPrice
    {
        public int Amount;
        public string Currency;

        internal static DiscordSkuPrice FromJSONNode(JSONNode n)
        {
            return new DiscordSkuPrice
            {
                Amount = n["amount"].AsInt,
                Currency = n["currency"].Value
            };
        }
    }

    [Serializable]
    public class DiscordSku
    {
        // Required
        public string Id;
        public string Name;
        public DiscordSkuType Type;
        public DiscordSkuPrice Price;
        public string ApplicationId;
        public int Flags;

        // Optional
        public string? ReleaseDate;

        internal static DiscordSku FromJSONNode(JSONNode n)
        {
            var rawType = n["type"].AsInt;
            if (!Enum.IsDefined(typeof(DiscordSkuType), rawType))
                throw new FormatException($"Unknown SkuType code: {rawType}");

            var sku = new DiscordSku
            {
                Id = n["id"].Value,
                Name = n["name"].Value,
                Type = (DiscordSkuType)rawType,
                Price = DiscordSkuPrice.FromJSONNode(n["price"]),
                ApplicationId = n["application_id"].Value,
                Flags = n["flags"].AsInt
            };

            if (n.HasKey("release_date"))
            {
                var rd = n["release_date"].Value;
                sku.ReleaseDate = string.IsNullOrEmpty(rd) ? null : rd;
            }

            return sku;
        }

        public static List<DiscordSku> FromJSON(string jsonString)
        {
            var list = new List<DiscordSku>();
            var root = JSON.Parse(jsonString);

            if (root.HasKey("skus") && root["skus"].IsArray)
            {
                foreach (var item in root["skus"].AsArray)
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
