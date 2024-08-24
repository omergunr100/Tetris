using System.Collections.Generic;
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
            HighScores = DiskMemoryManager.Instance.GetHighScores();
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
            DiskMemoryManager.Instance.SaveHighScore(Score);
            LoadHighScores();
            ResetScore();
        }
    }
}