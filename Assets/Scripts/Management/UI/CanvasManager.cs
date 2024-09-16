using UnityEngine;
using Utils;

namespace Management.UI
{
    public class CanvasManager : Singleton<CanvasManager>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas titleCanvas;
        [SerializeField] private Canvas endOfGameCanvas;
        [SerializeField] private Canvas inGameCanvas;

        private void Awake()
        {
            RegisterDelegates();
        }

        private void RegisterDelegates()
        {
            GameManager.Instance.AddGamePhaseListener(TitleCanvasSetEnabled);
            GameManager.Instance.AddGamePhaseListener(EndOfGameCanvasSetEnabled);
            GameManager.Instance.AddGamePhaseListener(InGameCanvasSetEnabled);
        }

        private void TitleCanvasSetEnabled(GamePhase phase) => titleCanvas.enabled = phase == GamePhase.Title;
        private void EndOfGameCanvasSetEnabled(GamePhase phase) => endOfGameCanvas.enabled = phase == GamePhase.Loss;
        private void InGameCanvasSetEnabled(GamePhase phase) => inGameCanvas.enabled = phase == GamePhase.Game;
    }
}