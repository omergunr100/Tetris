using System;

namespace Management.Playback.Actions
{
    public class GameAction : IGameAction
    {
        private readonly Action _action;

        public GameAction(Action action)
        {
            _action = action;
        }

        public void Play() => _action.Invoke();
    }
}