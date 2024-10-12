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
            titleCanvas.SetActive(phase is GamePhase.Title);
            endOfGameCanvas.SetActive(phase is GamePhase.Loss);
            inGameCanvas.SetActive(phase is GamePhase.Tetris or GamePhase.Tetrisd);
            highScoresCanvas.SetActive(phase is GamePhase.HighScores);
        }
    }
}