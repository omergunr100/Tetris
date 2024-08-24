using System.Collections.Generic;
using Utils;

namespace Management
{
    public class DiskMemoryManager : Singleton<DiskMemoryManager>
    {
        public List<int> GetHighScores()
        {
            // todo: load from disk
            return new List<int>();
        }

        public void SaveHighScore(int score)
        {
            // todo: save to disk
        }
    }
}