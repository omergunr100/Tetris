using Management.Score;
using UnityEngine;
using Utils;

namespace Management.UI
{
    public class CanvasManager : Singleton<CanvasManager>
    {
        [SerializeField] private GameObject titleCanvas;
        [SerializeField] private GameObject endOfGameCanvas;
        [SerializeField] private GameObject inGameCanvas;
        [SerializeField] private GameObject highScoresCanvas;

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
        }

        private void GamePhaseListener(GamePhase phase)
        {
            titleCanvas.SetActive(phase == GamePhase.Title);
            endOfGameCanvas.SetActive(phase == GamePhase.Loss);
            inGameCanvas.SetActive(phase == GamePhase.Tetris);
            highScoresCanvas.SetActive(phase == GamePhase.HighScores);
        }
    }
}