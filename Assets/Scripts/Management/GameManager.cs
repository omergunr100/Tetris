using System;
using System.Collections;
using System.Collections.Generic;
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
        
        private GamePhase CurrentGamePhase { get; set; } = GamePhase.Blank;
        private readonly List<Action<GamePhase>> _onPhaseChange = new();

        public void AddGamePhaseListener(Action<GamePhase> action) => _onPhaseChange.Add(action);

        public void setGamePhase(GamePhase newGamePhase)
        {
            CurrentGamePhase = newGamePhase;
            _onPhaseChange.ForEach(action => action.Invoke(newGamePhase));
            Debug.Log($"Current Game Phase: {newGamePhase}");
        }
        
        private void SaveScore()
        {
            ScoreManager.Instance.SaveScore();
        }

        private void Start()
        {
            setGamePhase(GamePhase.Title);
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
            // animate loss
            var blockScripts = FindObjectsByType<BlockScript>(FindObjectsSortMode.None);
            StartCoroutine(AnimateLoss(blockScripts));

            // clear managers
            TetrominoSpawner.Instance.Clear();
            BoardManager.Instance.Clear();
            
            // set game state to loss
            setGamePhase(GamePhase.Loss);
        }

        private IEnumerator AnimateLoss(BlockScript[] blockScripts)
        {
            yield break;
        }
    }
}