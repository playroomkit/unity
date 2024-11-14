using System;
using UBB;
using UnityEngine;

namespace Playroom
{
    public class BrowserMockPlayerService : PlayroomKit.Player.IPlayerBase
    {
        private readonly UnityBrowserBridge _ubb;
        private string _id;

        public BrowserMockPlayerService(UnityBrowserBridge ubb, string id)
        {
            _ubb = ubb;
            _id = id;
        }

        #region State

        public void SetState(string key, int value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, float value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, bool value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, string value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public void SetState(string key, object value, bool reliable = false)
        {
            _ubb.CallJs("SetPlayerStateByPlayerId", null, null, false, _id, key, value.ToString(),
                reliable.ToString().ToLower());
        }

        public T GetState<T>(string key)
        {
            Debug.Log(key);
            return _ubb.CallJs<T>(key);
        }

        #endregion

        public PlayroomKit.Player.Profile GetProfile()
        {
            string json = _ubb.CallJs<string>("GetProfile", null, null, false, _id);

            Debug.Log(json);
            
            var profileData = Helpers.ParseProfile(json);
            return profileData;
        }

        public Action OnQuit(Action<string> callback)
        {
            Debug.LogWarning("OnQuit not supported yet in Browser Mode");
            return default;
        }

        public void Kick(Action onKickCallBack = null)
        {
            _ubb.CallJs("Kick", null, null, true, _id);
        }

        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            _ubb.CallJs("WaitForPlayerState", null, null, true, _id, stateKey);
        }
    }
}