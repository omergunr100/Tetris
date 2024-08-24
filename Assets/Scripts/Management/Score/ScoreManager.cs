using System.Collections.Generic;
using Management.Disk;
using Utils;

namespace Management.Score
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        public int Score { get; private set; }
        public List<int> HighScores { get; private set; }

        private void Start()
        {
            LoadHighScores();
        }

        private void LoadHighScores()
        {
            // todo: read scores from file
            // HighScores = DiskMemoryManager.Instance.ReadFile();
        }
        
        public void AddScore(int score)
        {
            Score += score;
        }

        public void ResetScore()
        {
            Score = 0;
        }

        public void SaveScore()
        {
            // todo: write score to file
            // DiskMemoryManager.Instance.SaveHighScore(Score);
            LoadHighScores();
            ResetScore();
        }
    }
}