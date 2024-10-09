using System;
using System.Collections.Generic;
using Management.Playback.Actions;
using Utils;

namespace Management.Playback
{
    public class GameRecorder : Singleton<GameRecorder>
    {
        private readonly Queue<IGameAction> _gameActions = new();
        private bool _recording;

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(phase =>
            {
                _recording = phase == GamePhase.Tetris;
                if (phase == GamePhase.Tetris) _gameActions.Clear();
            });
        }

        public bool IsEmpty() => _gameActions.Count == 0;
        
        public bool GetNext(out IGameAction result)
        {
            result = null;
            if (!_gameActions.TryPeek(out result) || !result.ShouldPlay()) return false;
            _gameActions.Dequeue();
            return true;
        }

        public void RecordAction(IGameAction action) {
            if (_recording) _gameActions.Enqueue(action);
        }
    }
}