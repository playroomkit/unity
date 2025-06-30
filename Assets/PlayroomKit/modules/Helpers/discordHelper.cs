using System.Collections.Generic;
using SimpleJSON;

namespace Playroom
{
    public static class DiscordHelper
    {
        public static JSONNode SerializeDiscordOptions(object discord)
        {
            if (discord is bool booleanValue)
            {
                return booleanValue;
            }
            else if (discord is DiscordOptions discordOptions)
            {
                var node = new JSONObject();

                if (!string.IsNullOrEmpty(discordOptions.Prompt))
                    node["prompt"] = discordOptions.Prompt;

                if (discordOptions.Scope != null)
                {
                    var scopeArray = new JSONArray();
                    foreach (var s in discordOptions.Scope)
                    {
                        scopeArray.Add(s);
                    }
                    node["scope"] = scopeArray;
                }

                if (!string.IsNullOrEmpty(discordOptions.State))
                    node["state"] = discordOptions.State;

                return node;
            }
            else
            {
                // Not a supported type
                return JSON.Parse("{}");
            }
        }
    }
}
