using System;

namespace Playroom
{
    public interface IMockModeStrategy
    {
        void InsertCoin(PlayroomKit.InitOptions options, Action onLaunchCallBack);
    }
}