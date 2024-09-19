using System.Collections.Generic;
using Management;
using Management.Score;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HighScoresCanvas : MonoBehaviour
    {
        [SerializeField] private List<GameObject> rows;
        [SerializeField] private List<TextMeshProUGUI> names;
        [SerializeField] private List<TextMeshProUGUI> scores;

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
        }

        private void GamePhaseListener(GamePhase phase)
        {
            switch(phase)
            {
                case GamePhase.HighScores:
                    LoadHighScores();
                    break;
            }
        }

        private void LoadHighScores()
        {
            for (var i = 0; i < ScoreManager.Instance.HighScores.Count; i++)
            {
                var (playerName, score) = ScoreManager.Instance.HighScores[i];
                names[i].text = playerName;
                scores[i].text = score.ToString();
                rows[i].SetActive(true);
            }

            for (var i = ScoreManager.Instance.HighScores.Count; i < 5; i++)
                rows[i].SetActive(false);
        }

        public void BackToMenu() => GameManager.Instance.SetGamePhase(GamePhase.Title);
    }
}