namespace Playroom
{
    public interface IPlayroomBuildExtensions
    {
        public void SetState(string key, string value, bool reliable = false);
        void SetState(string key, int value, bool reliable = false);
        void SetState(string key, bool value, bool reliable = false);
        void SetState(string key, float value, bool reliable = false);
        void SetState(string key, object value, bool reliable = false);
    }
}