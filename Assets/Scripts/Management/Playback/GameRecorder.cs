using System.Collections.Generic;
using Management.Playback.Actions;
using UnityEngine;
using Utils;

namespace Management.Playback
{
    public class GameRecorder : Singleton<GameRecorder>
    {
        private readonly Queue<IGameAction> _gameActions = new();

        public float TimeSinceStart { get; private set; }
        
        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(phase =>
            {
                TimeSinceStart = 0;
                if (phase == GamePhase.Title) _gameActions.Clear();
            });
        }

        private void Update()
        {
            TimeSinceStart += Time.deltaTime;
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
            if (GameManager.Instance.CurrentGamePhase is GamePhase.Tetris) _gameActions.Enqueue(action);
        }
    }
}