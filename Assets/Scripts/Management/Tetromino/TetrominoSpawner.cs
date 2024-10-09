using System;
using System.Collections.Generic;
using Effects.Implementation;
using Management.Board;
using Management.Prefabs;
using Tetromino;
using UI;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Management.Tetromino
{
    public class TetrominoSpawner : Singleton<TetrominoSpawner>
    {
        private GameObject _effectHolder;
        
        private readonly Queue<TetrominoDefinition> _tetrominoQueue = new();

        private readonly List<Action<TetrominoDefinition>> _nextTetrominoChangeListeners = new();

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
        }

        private void Restock()
        {
            // add all tetrominos to the bucket in a random order
            var tetrominos = new List<TetrominoDefinition>(PrefabStore.Instance.tetrominoDataPrefabs);
            tetrominos.AddRange(PrefabStore.Instance.tetrominoDataPrefabs);
            while (tetrominos.Count > 0)
            {
                var randomIndex = Random.Range(0, tetrominos.Count);
                _tetrominoQueue.Enqueue(tetrominos[randomIndex]);
                tetrominos.RemoveAt(randomIndex);
            }
        }
        
        public void Spawn()
        {
            var definition = _tetrominoQueue.Dequeue();
            TetrominoScript.Instance.transform.position = BoardManager.Instance.SpawnPosition;
            TetrominoScript.Instance.Setup(definition);

            if (TitleCanvas.AdditionalContentEnabled()) ApplyRandomEffect();
            
            if (_tetrominoQueue.Count == 0) Restock();
            
            _nextTetrominoChangeListeners.ForEach(listener => listener.Invoke(_tetrominoQueue.Peek()));
        }

        private void GamePhaseListener(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Tetris:
                    _effectHolder = new GameObject("Effect Holder");
                    Restock();
                    break;
                case GamePhase.Loss:
                    _tetrominoQueue.Clear();
                    Destroy(_effectHolder);
                    break;
            }
        }

        public void AddNextTetrominoChangeListener(Action<TetrominoDefinition> listener) =>
            _nextTetrominoChangeListeners.Add(listener);

        private void ApplyRandomEffect()
        {
            var rand = Random.Range(0f, 1f);
            if (rand <= 0.05)
            {
                var bombBlock = TetrominoScript.Instance.ChooseRandomBlock();
                if (bombBlock == null) return;
                var bombEffect = _effectHolder.AddComponent<BombEffect>();
                bombEffect.Block = bombBlock;
            }
            else if (rand <= 0.25)
            {
                var speedUpBlock = TetrominoScript.Instance.ChooseRandomBlock();
                if (speedUpBlock == null) return;
                var speedUpEffect = _effectHolder.AddComponent<SpeedEffect>();
                speedUpEffect.Block = speedUpBlock;
            }
        }
    }
}