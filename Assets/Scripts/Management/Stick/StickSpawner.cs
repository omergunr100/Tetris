using Management.Board;
using UnityEngine;
using Utils;

namespace Management.Stick
{
    public class StickSpawner : Singleton<StickSpawner>
    {
        [SerializeField] private GameObject stickFigure;

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(phase =>
            {
                if (phase == GamePhase.Tetrisd)
                    SpawnStick();
                else if (stickFigure != null)
                    stickFigure.SetActive(false);
            });
        }

        private void SpawnStick()
        {
            Debug.Log("Moving stick figure to position...");
            stickFigure.transform.position = BoardManager.Instance.StickSpawnPosition + Vector3.up * 5;
            stickFigure.SetActive(true);
            Debug.Log("Moved and activated stick figure");
        }
    }
}