using System.Collections.Generic;
using Management.Pooling;
using Management.Tetromino;
using Tetromino;
using UnityEngine;

namespace Management.Board
{
    public class UpcomingTetrominos : MonoBehaviour
    {
        private readonly List<BlockScript> _currentBlocks = new();
        
        private void Awake()
        {
            TetrominoSpawner.Instance.AddNextTetrominoChangeListener(NextTetrominoChangeListener);
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
        }

        private void NextTetrominoChangeListener(TetrominoDefinition definition)
        {
            Clear();
            gameObject.transform.position = BoardManager.Instance.GetUpcomingPosition(definition);
            TetrominoDefinition.Setup(definition, _currentBlocks, gameObject);
        } 
        
        private void GamePhaseListener(GamePhase phase)
        {
            switch(phase)
            {
                case GamePhase.Loss:
                    Clear();
                    break;
            }
        }

        private void Clear()
        {
            _currentBlocks.ForEach(block => PoolStore.Instance.Release(block));
            _currentBlocks.Clear();
        }
    }
}