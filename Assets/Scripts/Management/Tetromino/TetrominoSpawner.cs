using System;
using System.Collections.Generic;
using Effects.Definition;
using Effects.Implementation;
using Management.Board;
using Management.Playback;
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
            SpawnDefinition(definition);
            RecorderUtils.RecordSpawn(definition);

            if (TitleCanvas.AdditionalContentEnabled())
            {
                if (RollRandomEffect(out var effect))
                {
                    var index = TetrominoScript.Instance.ChooseRandomBlockIndex();
                    ApplyEffect(effect, index);
                    RecorderUtils.RecordEffect(effect, index);
                }
            }
            
            if (_tetrominoQueue.Count == 0) Restock();
            
            _nextTetrominoChangeListeners.ForEach(listener => listener.Invoke(_tetrominoQueue.Peek()));
        }

        public void SpawnDefinition(TetrominoDefinition definition)
        {
            TetrominoScript.Instance.transform.position = BoardManager.Instance.TetrominoSpawnPosition;
            TetrominoScript.Instance.Setup(definition);
        }
        
        public void ApplyEffect(Type effect, int index)
        {
            var blocks = TetrominoScript.Instance.TetrominoBlocks;
            if (!typeof(BaseEffect).IsAssignableFrom(effect) || blocks.Count <= index || index <= 0) return;
            var effectComp = (BaseEffect)_effectHolder.AddComponent(effect);
            effectComp.Block = blocks[index];
        }

        private void GamePhaseListener(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Tetris:
                    _effectHolder = new GameObject("Effect Holder");
                    Restock();
                    break;
                case GamePhase.Tetrisd:
                    Destroy(_effectHolder);
                    _effectHolder = new GameObject("Effect Holder");
                    break;
                case GamePhase.Loss:
                    _tetrominoQueue.Clear();
                    Destroy(_effectHolder);
                    break;
            }
        }

        public void AddNextTetrominoChangeListener(Action<TetrominoDefinition> listener) =>
            _nextTetrominoChangeListeners.Add(listener);

        private static bool RollRandomEffect(out Type effect)
        {
            effect = null;
            var rand = Random.Range(0f, 1f);
            if (rand <= 0.05)
            {
                effect = typeof(BombEffect);
            }
            else if (rand <= 0.25)
            {
                effect = typeof(SpeedEffect);
            }

            return effect != null;
        }
    }
}