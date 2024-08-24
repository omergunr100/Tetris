using System;
using System.Collections.Generic;
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
        [SerializeField] private MonoBehaviour[] managers;
        
        private float _gameSpeedModifier;
        private float _timeSinceStart;
        public readonly List<TetrominoScript> CurrentTetrominos = new();

        private float _timeSinceDrop;

        public float GameSpeed => baseGameSpeed + _gameSpeedModifier;
        
        public GamePhase Phase { get; private set; }
        
        private void Awake()
        {
            ResetState();
        }

        private void Start()
        {
            Phase = GamePhase.TitleScreen;
        }

        private void ResetState()
        {
        }
        
        private void SaveScore()
        {
            ScoreManager.Instance.SaveScore();
        }

        public void MoveTetrominos(Vector3 offset)
        {
            CurrentTetrominos.ForEach(tetromino => tetromino.Move(offset));
        }
        
        public void RotateTetrominos()
        {
            CurrentTetrominos.ForEach(tetromino => tetromino.Rotate());
        }

        private void Update()
        {
            _timeSinceStart += Time.deltaTime;
            _timeSinceDrop += Time.deltaTime;
            
            if (CurrentTetrominos.Count == 0)
                TetrominoSpawner.Instance.Spawn();

            if (_timeSinceDrop >= 1f / GameSpeed)
            {
                CurrentTetrominos.ForEach(tetromino => tetromino.Move(Vector3.down));
                _timeSinceDrop = 0f;
            }
        }

        public void OnLoss()
        {
            
        }
    }
}