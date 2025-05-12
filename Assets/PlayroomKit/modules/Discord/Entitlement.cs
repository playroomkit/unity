using System;
using System.Collections.Generic;
using SimpleJSON;

public enum EntitlementType
{
    Unhandled = -1,
    Purchase = 1,
    PremiumSubscription = 2,
    DeveloperGift = 3,
    TestModePurchase = 4,
    FreePurchase = 5,
    UserGift = 6,
    PremiumPurchase = 7
}

[Serializable]
public class Entitlement
{
    public string Id;
    public string SkuId;
    public string ApplicationId;
    public string UserId;
    public int GiftCodeFlags;
    public EntitlementType Type;
    public string? GifterUserId;
    public List<string>? Branches;
    public DateTime? StartsAt;
    public DateTime? EndsAt;
    public string? ParentId;
    public bool? Consumed;
    public bool? Deleted;
    public string? GiftCodeBatchId;

    /// <summary>
    /// Parse a JSONNode (from SimpleJSON) into an Entitlement instance.
    /// </summary>
    public static Entitlement FromJSON(JSONNode n)
    {
        var e = new Entitlement();

        e.Id = n["id"] ?? throw new Exception("id is required");
        e.SkuId = n["sku_id"] ?? throw new Exception("sku_id is required");
        e.ApplicationId = n["application_id"] ?? throw new Exception("application_id is required");
        e.UserId = n["user_id"] ?? throw new Exception("user_id is required");
        e.GiftCodeFlags = n["gift_code_flags"].AsInt;
        e.Type = (EntitlementType)n["type"].AsInt;

        // optional / nullable
        e.GifterUserId = n["gifter_user_id"].IsNull ? null : n["gifter_user_id"];

        if (n["branches"].IsArray)
        {
            e.Branches = new List<string>();
            foreach (var item in n["branches"].AsArray)
                e.Branches.Add(item.Value);
        }

        // parse ISO date strings if present & non-null
        string sa = n["starts_at"];
        e.StartsAt = string.IsNullOrEmpty(sa) ? (DateTime?)null : DateTime.Parse(sa, null, System.Globalization.DateTimeStyles.RoundtripKind);

        string ea = n["ends_at"];
        e.EndsAt = string.IsNullOrEmpty(ea) ? (DateTime?)null : DateTime.Parse(ea, null, System.Globalization.DateTimeStyles.RoundtripKind);

        e.ParentId = n["parent_id"].IsNull ? null : n["parent_id"];
        e.Consumed = n["consumed"].IsNull ? (bool?)null : n["consumed"].AsBool;
        e.Deleted = n["deleted"].IsNull ? (bool?)null : n["deleted"].AsBool;
        e.GiftCodeBatchId = n["gift_code_batch_id"].IsNull ? null : n["gift_code_batch_id"];

        return e;
    }

}
