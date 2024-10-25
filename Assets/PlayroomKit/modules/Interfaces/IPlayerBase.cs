namespace Playroom
{
    public interface IPlayerBase
    {
        void SetState(string id, string key, object value, bool reliable = false);
        public T GetState<T>(string id, string key);
    }
}