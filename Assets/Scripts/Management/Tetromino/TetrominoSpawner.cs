using System;
using System.Collections.Generic;
using Effects.Implementation;
using Management.Board;
using Management.Prefabs;
using Tetromino;
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
            _effectHolder = new GameObject("Effect Holder");
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
            var bombBlock = TetrominoScript.Instance.TetrominoBlocks[Random.Range(0, TetrominoScript.Instance.TetrominoBlocks.Count)];
            var bombEffect = _effectHolder.AddComponent<BombEffect>();
            bombEffect.Block = bombBlock;

            if (_tetrominoQueue.Count == 0) Restock();
            
            _nextTetrominoChangeListeners.ForEach(listener => listener.Invoke(_tetrominoQueue.Peek()));
        }

        private void GamePhaseListener(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Game:
                    Restock();
                    break;
                case GamePhase.Loss:
                    _tetrominoQueue.Clear();
                    break;
            }
        }

        public void AddNextTetrominoChangeListener(Action<TetrominoDefinition> listener) =>
            _nextTetrominoChangeListeners.Add(listener);
    }
}