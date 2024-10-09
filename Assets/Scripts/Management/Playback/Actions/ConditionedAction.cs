using System;

namespace Management.Playback.Actions
{
    public class ConditionedAction : IGameAction
    {
        private readonly Func<bool> _condition;
        private readonly IGameAction _gameAction;

        public ConditionedAction(Func<bool> condition, IGameAction action)
        {
            _condition = condition;
            _gameAction = action;
        }

        public void Play() => _gameAction.Play();
        public bool ShouldPlay() => _condition.Invoke();
    }
}