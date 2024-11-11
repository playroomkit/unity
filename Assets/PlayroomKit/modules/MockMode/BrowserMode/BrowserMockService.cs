using System;

namespace Playroom
{
    public class BrowserMockService : PlayroomKit.IPlayroomBase
    {
        #region Initialization Methods

        public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
            Action onDisconnectCallback = null)
        {
            throw new NotImplementedException();
        }

        public Action OnPlayerJoin(Action<PlayroomKit.Player> onPlayerJoinCallback)
        {
            throw new NotImplementedException();
        }

        public void StartMatchmaking(Action callback = null)
        {
            throw new NotImplementedException();
        }

        public void OnDisconnect(Action callback)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Local Player Mehtods

        public PlayroomKit.Player MyPlayer()
        {
            throw new NotImplementedException();
        }

        public PlayroomKit.Player Me()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Room Mehtods

        public bool IsHost()
        {
            throw new NotImplementedException();
        }

        public string GetRoomCode()
        {
            throw new NotImplementedException();
        }

        public bool IsStreamScreen()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region State Syncing Methods

        public void SetState<T>(string key, T value, bool reliable = false)
        {
            throw new NotImplementedException();
        }

        public T GetState<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void WaitForState(string stateKey, Action<string> onStateSetCallback = null)
        {
            throw new NotImplementedException();
        }

        public void WaitForPlayerState(string playerID, string stateKey, Action<string> onStateSetCallback = null)
        {
            throw new NotImplementedException();
        }

        public void ResetStates(string[] keysToExclude = null, Action onStatesReset = null)
        {
            throw new NotImplementedException();
        }

        public void ResetPlayersStates(string[] keysToExclude = null, Action onStatesReset = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Joystick Mehtods

        public void CreateJoyStick(PlayroomKit.JoystickOptions options)
        {
            throw new NotImplementedException();
        }

        public PlayroomKit.Dpad DpadJoystick()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Misc

        public void UnsubscribeOnQuit()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}