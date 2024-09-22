using Management;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TitleCanvas : MonoBehaviour
    {
        private const string AdditionalContentKey = "AdditionalContentSetting"; 
        
        [SerializeField] private Image additionalContentImage; 
        
        private void Awake()
        {
            if (!PlayerPrefs.HasKey(AdditionalContentKey)) PlayerPrefs.SetInt(AdditionalContentKey, 0);
            SetColor();
        }
        
        private void SetColor() => 
            additionalContentImage.color = PlayerPrefs.GetInt(AdditionalContentKey) == 0 ? Color.red : Color.green; 

        public void StartGame() => GameManager.Instance.SetGamePhase(GamePhase.Game);

        public void SwitchContentSetting()
        {
            var contentSettingValue = PlayerPrefs.GetInt(AdditionalContentKey);
            contentSettingValue = contentSettingValue == 0 ? 1 : 0;
            PlayerPrefs.SetInt(AdditionalContentKey, contentSettingValue);
            SetColor();
        }

        public void OpenHighScores() => GameManager.Instance.SetGamePhase(GamePhase.HighScores);

        public static bool AdditionalContentEnabled => PlayerPrefs.HasKey(AdditionalContentKey) && PlayerPrefs.GetInt(AdditionalContentKey) != 0;
    }
}