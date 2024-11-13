
using System;

namespace Playroom
{
    public class BrowserMockPlayerService : PlayroomKit.Player.IPlayerBase
    {
        
        #region State 
        public void SetState(string key, int value, bool reliable = false)
        {
            throw new NotImplementedException();
        }

        public void SetState(string key, float value, bool reliable = false)
        {
            throw new NotImplementedException();
        }

        public void SetState(string key, bool value, bool reliable = false)
        {
            throw new NotImplementedException();
        }

        public void SetState(string key, string value, bool reliable = false)
        {
            throw new NotImplementedException();
        }

        public void SetState(string key, object value, bool reliable = false)
        {
            throw new NotImplementedException();
        }

        public T GetState<T>(string key)
        {
            throw new NotImplementedException();
        }
        #endregion

        public PlayroomKit.Player.Profile GetProfile()
        {
            throw new NotImplementedException();
        }

        public Action OnQuit(Action<string> callback)
        {
            throw new NotImplementedException();
        }

        public void Kick(Action onKickCallBack = null)
        {
            throw new NotImplementedException();
        }

        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            throw new NotImplementedException();
        }
    }
}