using System;
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
        
        private static float ScoreSpeedModifier => Mathf.Log(ScoreManager.Instance.Score + 1, 100);
        public float AdditionalSpeedModifier { get; set; }

        private float _timeSinceDrop;

        public float GameSpeed() => baseGameSpeed + ScoreSpeedModifier + AdditionalSpeedModifier;
        
        private GamePhase CurrentGamePhase { get; set; } = GamePhase.Blank;
        private readonly List<Action<GamePhase>> _onPhaseChange = new();

        public void AddGamePhaseListener(Action<GamePhase> action) => _onPhaseChange.Add(action);
        public void RemoveGamePhaseListener(Action<GamePhase> action) => _onPhaseChange.Remove(action);

        public void SetGamePhase(GamePhase newGamePhase)
        {
            CurrentGamePhase = newGamePhase;
            _onPhaseChange.ForEach(action => action.Invoke(newGamePhase));
            Debug.Log($"Current Game Phase: {newGamePhase}");
        }

        private void Awake()
        {
            AddGamePhaseListener(GamePhaseListener);
        }

        private void Start()
        {
            SetGamePhase(GamePhase.Title);
        }

        private void Update()
        {
            if (CurrentGamePhase == GamePhase.Game)
            {
                _timeSinceDrop += Time.deltaTime;
                
                if (TetrominoScript.Instance.Removed)
                    TetrominoSpawner.Instance.Spawn();
                else if (_timeSinceDrop >= 1f / GameSpeed())
                {
                    BoardManager.Instance.TryMove(Vector3.down);
                    _timeSinceDrop = 0f;
                }
            }
        }

        private void GamePhaseListener(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Title:
                    break;
                case GamePhase.Game:
                    _timeSinceDrop = 0;
                    AdditionalSpeedModifier = 0;
                    break;
                case GamePhase.Loss:
                    break;
                case GamePhase.HighScores:
                    break;
            }
        }
    }
}