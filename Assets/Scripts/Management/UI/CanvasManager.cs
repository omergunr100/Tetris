using UnityEngine;
using Utils;

namespace Management.UI
{
    public class CanvasManager : Singleton<CanvasManager>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Canvas titleCanvas;
        [SerializeField] private Canvas endOfGameCanvas;

        private void Awake()
        {
            RegisterDelegates();
        }

        private void RegisterDelegates()
        {
            GameManager.Instance.AddGamePhaseListener(TitleCanvasSetEnabled);
            GameManager.Instance.AddGamePhaseListener(EndOfGameCanvasSetEnabled);
        }

        private void TitleCanvasSetEnabled(GamePhase phase) => titleCanvas.enabled = phase == GamePhase.Title;
        private void EndOfGameCanvasSetEnabled(GamePhase phase) => endOfGameCanvas.enabled = phase == GamePhase.Loss;
    }
}