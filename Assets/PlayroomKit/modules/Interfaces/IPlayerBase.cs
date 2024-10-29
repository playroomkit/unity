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
                void SetState(string id, string key, object value, bool reliable = false);
                public T GetState<T>(string id, string key);

                public Profile GetProfile(string id);
                
            }
        }
    }
}
