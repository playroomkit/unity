using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    /// <summary>
    /// This file contains functions, mostly used for serialization / deserialization 
    /// </summary>
    public class Helpers
    {
        public static string SerializeInitOptions(InitOptions options)
        {
            if (options == null) return null;

            JSONNode node = new JSONObject();

            node["streamMode"] = options.streamMode;
            node["allowGamepads"] = options.allowGamepads;
            node["baseUrl"] = options.baseUrl;

            if (options.avatars != null)
            {
                JSONArray avatarsArray = new JSONArray();
                foreach (string avatar in options.avatars)
                {
                    avatarsArray.Add(avatar);
                }

                node["avatars"] = avatarsArray;
            }

            node["roomCode"] = options.roomCode;
            node["skipLobby"] = options.skipLobby;
            node["reconnectGracePeriod"] = options.reconnectGracePeriod;

            // Serialize matchmaking field
            if (options.matchmaking is bool booleanMatchmaking)
            {
                node["matchmaking"] = booleanMatchmaking;
            }
            else if (options.matchmaking is MatchMakingOptions matchmakingOptions)
            {
                JSONNode matchmakingNode = new JSONObject();
                matchmakingNode["waitBeforeCreatingNewRoom"] = matchmakingOptions.waitBeforeCreatingNewRoom;
                node["matchmaking"] = matchmakingNode;
            }

            if (options.maxPlayersPerRoom.HasValue)
            {
                node["maxPlayersPerRoom"] = options.maxPlayersPerRoom.Value;
            }

            if (options.gameId != null)
            {
                node["gameId"] = options.gameId;
            }

            node["discord"] = options.discord;
            node["persistentMode"] = options.persistentMode;

            if (options.defaultStates != null)
            {
                JSONObject defaultStatesObject = new JSONObject();
                foreach (var kvp in options.defaultStates)
                {
                    defaultStatesObject[kvp.Key] = ConvertValueToJSON(kvp.Value);
                }

                node["defaultStates"] = defaultStatesObject;
            }

            if (options.defaultPlayerStates != null)
            {
                JSONObject defaultPlayerStatesObject = new JSONObject();
                foreach (var kvp in options.defaultPlayerStates)
                {
                    defaultPlayerStatesObject[kvp.Key] = ConvertValueToJSON(kvp.Value);
                }

                node["defaultPlayerStates"] = defaultPlayerStatesObject;
            }


            DebugLogger.LogWarning(node.ToString());

            return node.ToString();
        }

        private static JSONNode ConvertValueToJSON(object value)
        {
            if (value is string stringValue)
            {
                return stringValue;
            }
            else if (value is int intValue)
            {
                return intValue;
            }
            else if (value is float floatValue)
            {
                return floatValue;
            }
            else if (value is bool boolValue)
            {
                return boolValue;
            }
            else
            {
                // Handle other types if needed
                return JSON.Parse("{}");
            }
        }

        public static JSONArray CreateJsonArray(string[] array)
        {
            JSONArray jsonArray = new JSONArray();

            foreach (string item in array)
            {
                jsonArray.Add(item);
            }

            return jsonArray;
        }

        public static string ConvertJoystickOptionsToJson(JoystickOptions options)
        {
            JSONNode joystickOptionsJson = new JSONObject();
            joystickOptionsJson["type"] = options.type;

            // Serialize the buttons array
            JSONArray buttonsArray = new JSONArray();
            foreach (ButtonOptions button in options.buttons)
            {
                JSONObject buttonJson = new JSONObject();
                buttonJson["id"] = button.id;
                buttonJson["label"] = button.label;
                buttonJson["icon"] = button.icon;
                buttonsArray.Add(buttonJson);
            }

            joystickOptionsJson["buttons"] = buttonsArray;

            // Serialize the zones property (assuming it's not null)
            if (options.zones != null)
            {
                JSONObject zonesJson = new JSONObject();
                zonesJson["up"] = ConvertButtonOptionsToJson(options.zones.up);
                zonesJson["down"] = ConvertButtonOptionsToJson(options.zones.down);
                zonesJson["left"] = ConvertButtonOptionsToJson(options.zones.left);
                zonesJson["right"] = ConvertButtonOptionsToJson(options.zones.right);
                joystickOptionsJson["zones"] = zonesJson;
            }

            joystickOptionsJson["keyboard"] = options.keyboard;

            return joystickOptionsJson.ToString();
        }

        // Function to convert ButtonOptions to JSON
        private static JSONNode ConvertButtonOptionsToJson(ButtonOptions button)
        {
            JSONObject buttonJson = new JSONObject();
            buttonJson["id"] = button.id;
            buttonJson["label"] = button.label;
            buttonJson["icon"] = button.icon;
            return buttonJson;
        }

        public static PlayroomKit.Player.Profile ParseProfile(string json)
        {
            var jsonNode = JSON.Parse(json);
            var profileData = new PlayroomKit.Player.Profile();
            profileData.playerProfileColor = new PlayroomKit.Player.Profile.PlayerProfileColor
            {
                r = jsonNode["color"]["r"].AsInt,
                g = jsonNode["color"]["g"].AsInt,
                b = jsonNode["color"]["b"].AsInt,
                hexString = jsonNode["color"]["hexString"].Value,
                hex = jsonNode["color"]["hex"].AsInt
            };

            ColorUtility.TryParseHtmlString(profileData.playerProfileColor.hexString, out UnityEngine.Color color1);
            profileData.color = color1;
            profileData.name = jsonNode["name"].Value;
            profileData.photo = jsonNode["photo"].Value;

            return profileData;
        }

        private static Dictionary<string, T> ParseJsonToDictionary<T>(string jsonString)
        {
            var dictionary = new Dictionary<string, T>();
            var jsonNode = JSON.Parse(jsonString);

            foreach (var kvp in jsonNode.AsObject)
            {
                T value = default; // Initialize the value to default value of T

                // Parse the JSONNode value to the desired type (T)
                if (typeof(T) == typeof(float))
                    value = (T)(object)kvp.Value.AsFloat;
                else if (typeof(T) == typeof(int))
                    value = (T)(object)kvp.Value.AsInt;
                else if (typeof(T) == typeof(bool))
                    value = (T)(object)kvp.Value.AsBool;
                else
                    Debug.LogError("Unsupported type: " + typeof(T).FullName);

                dictionary.Add(kvp.Key, value);
            }

            return dictionary;
        }

        public static string SerializeObject(object value)
        {
            if (value == null) return JSONNull.CreateOrGet();

            switch (value)
            {
                case int i: return new JSONNumber(i);
                case float f: return new JSONNumber(f);
                case bool b: return new JSONBool(b);
                case string s: return value.ToString();
                case IDictionary dictionary:
                    var jsonDict = new JSONObject();
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        jsonDict[entry.Key.ToString()] = ConvertValueToJSON(entry.Value);
                    }

                    return jsonDict;
                case IEnumerable list:
                    var jsonArray = new JSONArray();
                    foreach (var item in list)
                    {
                        jsonArray.Add(ConvertValueToJSON(item));
                    }

                    return jsonArray;
                default:
                    Debug.LogError($"{value.GetType()} requires manual serialization!");
                    return default;
            }
        }
    }
}