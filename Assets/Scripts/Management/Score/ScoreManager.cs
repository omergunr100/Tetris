using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Management.Score
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        public int Score { get; private set; }
        
        private readonly string _playerPrefsPrefix = "HighScore";
        private readonly string _playerNamePostfix = "Name";
        private readonly string _playerScorePostfix = "Score";

        private readonly List<Action<int>> _scoreChangeListeners = new();
        public readonly List<(string, int)> HighScores = new();

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
            LoadHighScores();
        }

        public void AddScoreChangeListener(Action<int> listener) => _scoreChangeListeners.Add(listener);

        private void LoadHighScores()
        {
            HighScores.Clear();
            for (var i = 0; i < 5 && PlayerPrefs.HasKey(_playerPrefsPrefix + i + _playerNamePostfix); i++)
            {
                HighScores.Add((PlayerPrefs.GetString(_playerPrefsPrefix + i + _playerNamePostfix), 
                    PlayerPrefs.GetInt(_playerPrefsPrefix + i + _playerScorePostfix)));
            }
        }

        private void SaveScores()
        {
            // save all current high scores
            for (var i = 0; i < HighScores.Count; i++)
            {
                var (playerName, score) = HighScores[i];
                PlayerPrefs.SetString(_playerPrefsPrefix + i + _playerNamePostfix, playerName);
                PlayerPrefs.SetInt(_playerPrefsPrefix + i + _playerScorePostfix, score);
            }

            for (var i = HighScores.Count; i < 5; i++)
            {
                PlayerPrefs.DeleteKey(_playerPrefsPrefix + i + _playerNamePostfix);
                PlayerPrefs.DeleteKey(_playerPrefsPrefix + i + _playerScorePostfix);
            }
            
            PlayerPrefs.Save();
        }

        private int LowestHighScore => HighScores[^1].Item2;

        public bool IsHighScore() => Score > 0 && (HighScores.Count < 5 || Score > LowestHighScore);
        
        public void AddHighScore(string playerName)
        {
            for (var i = 0; i < 5; i++)
            {
                if (HighScores.Count >= i + 1 && HighScores[i].Item2 >= Score) continue;
                HighScores.Insert(i, (playerName, Score));
                break;
            }

            if (HighScores.Count > 5) HighScores.RemoveAt(5);
            
            SaveScores();
        }

        private void SetScore(int newScore)
        {
            Score = newScore;
            _scoreChangeListeners.ForEach(listener => listener.Invoke(newScore));
            Debug.Log($"New score is {Score}");
        }

        private void GamePhaseListener(GamePhase phase)
        {
            if (phase == GamePhase.Game)
                SetScore(0);
        }

        public void AddScore(int score) => SetScore(Score + score);
    }
}