using System.Collections;
using Management.Board;
using Management.Score;
using Management.Tetromino;
using Tetromino;
using UnityEngine;
using Utils;

namespace Management
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private float baseGameSpeed = 1f;
        
        private float _gameSpeedModifier;
        private float _timeSinceStart;

        private float _timeSinceDrop;

        public float GameSpeed => baseGameSpeed + _gameSpeedModifier;

        public GamePhase CurrentGamePhase { get; private set; } = GamePhase.Game; // todo: change to title screen
        
        private void SaveScore()
        {
            ScoreManager.Instance.SaveScore();
        }

        private void Update()
        {
            if (CurrentGamePhase == GamePhase.Game)
            {
                _timeSinceStart += Time.deltaTime;
                _timeSinceDrop += Time.deltaTime;
                
                if (TetrominoScript.Instance.Removed)
                    TetrominoSpawner.Instance.Spawn();
                else if (_timeSinceDrop >= 1f / GameSpeed)
                {
                    BoardManager.Instance.TryMove(Vector3.down);
                    _timeSinceDrop = 0f;
                }
            }
        }

        public void OnLoss()
        {
            // set game state to loss
            CurrentGamePhase = GamePhase.Loss;

            // animate loss
            var blockScripts = FindObjectsByType<BlockScript>(FindObjectsSortMode.None);
            StartCoroutine(AnimateLoss(blockScripts));

            // clear managers
            TetrominoSpawner.Instance.Clear();
            BoardManager.Instance.Clear();
        }

        public void Clear()
        {
        }

        private IEnumerator AnimateLoss(BlockScript[] blockScripts)
        {
            yield break;
        }
    }
}