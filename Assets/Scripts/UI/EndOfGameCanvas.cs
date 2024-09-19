using Management;
using Management.Score;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EndOfGameCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreTitle;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI backToTitleText;
        [SerializeField] private TMP_InputField playerNameField;

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
            playerNameField.characterLimit = 4;
        }

        private void GamePhaseListener(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Loss:
                    scoreTitle.text = "Score:" + (ScoreManager.Instance.IsHighScore() ? " (High Score!)" : "");
                    scoreText.text = ScoreManager.Instance.Score.ToString();
                    playerNameField.gameObject.SetActive(ScoreManager.Instance.IsHighScore());
                    playerNameField.text = "";
                    backToTitleText.text =
                        "Back To Menu" + (ScoreManager.Instance.IsHighScore() ? " (And Save Score)" : "");
                    break;
            }
        }

        public void GoBackToTitle()
        {
            if (ScoreManager.Instance.IsHighScore() && playerNameField.text != "")
                ScoreManager.Instance.AddHighScore(playerNameField.text);

            GameManager.Instance.SetGamePhase(GamePhase.Title);
        }
    }
}