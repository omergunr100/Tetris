using System;

namespace Management.Playback.Actions
{
    public class DataAction<T> : IGameAction
    {
        private readonly float _time;
        private readonly T _data;
        private readonly Action<T> _action;

        public DataAction(T data, Action<T> action)
        {
            _time = GameRecorder.Instance.TimeSinceStart;
            _data = data;
            _action = action;
        }
        
        public void Play() => _action.Invoke(_data);

        public float GetTime() => _time;
    }
}