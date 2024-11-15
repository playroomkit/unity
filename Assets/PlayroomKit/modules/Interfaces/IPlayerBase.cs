using System;
using UnityEngine.Serialization;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public partial class Player
        {
            public interface IPlayerBase
            {
                void SetState(string key, int value, bool reliable = false);
                void SetState(string key, float value, bool reliable = false);
                void SetState(string key, bool value, bool reliable = false);
                void SetState(string key, string value, bool reliable = false);
                void SetState(string key, object value, bool reliable = false);
                public T GetState<T>(string key);
                public Profile GetProfile();
                public Action OnQuit(Action<string> callback);
                public void Kick(Action onKickCallBack = null);

                public void WaitForState(string stateKey, Action<string> onStateSetCallback = null);

                protected static Action onKickCallBack = null;
            }
        }
    }
}