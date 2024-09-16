using System;
using System.Collections.Generic;
using Utils;

namespace Management.Score
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        private int _score;
        public List<int> HighScores { get; private set; }

        private List<Action<int>> _scoreChangeListeners = new();

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(Reset);
        }

        private void Start()
        {
            LoadHighScores();
        }

        public void AddScoreChangeListener(Action<int> listener) => _scoreChangeListeners.Add(listener);
        
        private void LoadHighScores()
        {
            // todo: read scores from file
            // HighScores = DiskMemoryManager.Instance.ReadFile();
        }
        
        public void AddScore(int score) => SetScore(_score + score);

        public void SaveScore()
        {
            // todo: write score to file
            // DiskMemoryManager.Instance.SaveHighScore(Score);
            LoadHighScores();
        }

        private void Reset(GamePhase phase)
        {
            if (phase != GamePhase.Loss)
                SetScore(0);
        }

        private void SetScore(int newScore)
        {
            _score = newScore;
            _scoreChangeListeners.ForEach(listener => listener.Invoke(newScore));
        }
    }
}