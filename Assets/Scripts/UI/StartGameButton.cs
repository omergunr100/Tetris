using Management;
using UnityEngine;

namespace UI
{
    public class StartGameButton : MonoBehaviour
    {
        public void SetGameStateToGame() => GameManager.Instance.setGamePhase(GamePhase.Game);
    }
}