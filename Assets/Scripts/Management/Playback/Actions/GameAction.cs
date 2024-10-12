using System;

namespace Management.Playback.Actions
{
    public class GameAction : IGameAction
    {
        private readonly float _time;
        private readonly Action _action;

        public GameAction(Action action)
        {
            _time = GameRecorder.Instance.TimeSinceStart;
            _action = action;
        }

        public float GetTime() => _time;

        public void Play() => _action.Invoke();
    }
}