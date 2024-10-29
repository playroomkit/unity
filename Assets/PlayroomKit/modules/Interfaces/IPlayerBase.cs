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
                void SetState(string id, string key, int value, bool reliable = false);
                void SetState(string id, string key, float value, bool reliable = false);
                void SetState(string id, string key, bool value, bool reliable = false);
                void SetState(string id, string key, string value, bool reliable = false);
                void SetState(string id, string key, object value, bool reliable = false);
                public T GetState<T>(string id, string key);
                public Profile GetProfile(string id);
                public Action OnQuit(Action<string> callback);
                public void Kick(string id, Action OnKickCallBack = null);
                
                protected static Action onKickCallBack = null;

            }
        }
    }
}
