using System;
using System.Collections.Generic;
using SimpleJSON;

namespace Discord
{
    public enum SkuType
    {
        UNHANDLED = -1,
        APPLICATION = 1,
        DLC = 2,
        CONSUMABLE = 3,
        BUNDLE = 4,
        SUBSCRIPTION = 5
    }

    [Serializable]
    public class SkuPrice
    {
        public int Amount;
        public string Currency;

        internal static SkuPrice FromJSONNode(JSONNode n)
        {
            return new SkuPrice
            {
                Amount = n["amount"].AsInt,
                Currency = n["currency"].Value
            };
        }
    }

    [Serializable]
    public class Sku
    {
        // Required
        public string Id;
        public string Name;
        public SkuType Type;
        public SkuPrice Price;
        public string ApplicationId;
        public int Flags;

        // Optional
        public string? ReleaseDate;   

        internal static Sku FromJSONNode(JSONNode n)
        {
            var rawType = n["type"].AsInt;
            if (!Enum.IsDefined(typeof(SkuType), rawType))
                throw new FormatException($"Unknown SkuType code: {rawType}");

            var sku = new Sku
            {
                Id = n["id"].Value,
                Name = n["name"].Value,
                Type = (SkuType)rawType,
                Price = SkuPrice.FromJSONNode(n["price"]),
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

        public static List<Sku> FromJSON(string jsonString)
        {
            var list = new List<Sku>();
            var root = JSON.Parse(jsonString);

            if (root.IsArray)
                foreach (var item in root.AsArray)
                    list.Add(FromJSONNode(item));
            else
                list.Add(FromJSONNode(root));

            return list;
        }
    }
}
