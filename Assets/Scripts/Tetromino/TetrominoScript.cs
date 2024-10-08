﻿using System;
using System.Collections.Generic;
using Management;
using Management.Board;
using Management.Pooling;
using Utils;
using Random = UnityEngine.Random;

namespace Tetromino
{
    public class TetrominoScript : Singleton<TetrominoScript>
    {
        public TetrominoDefinition Definition { get; private set; } 
        public readonly List<BlockScript> TetrominoBlocks = new();
        public bool Removed => TetrominoBlocks.Count == 0;

        private void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(phase =>
            {
                switch (phase)
                {
                    case GamePhase.Loss:
                        TetrominoBlocks.ForEach(block =>
                        {
                            if (block.gameObject.activeSelf)
                                PoolStore.Instance.Release(block);
                        });
                        TetrominoBlocks.Clear();
                        break;
                }
            });
        }

        public void Setup(TetrominoDefinition definition)
        {
            Definition = definition;
            TetrominoDefinition.Setup(definition, TetrominoBlocks, gameObject);
        }

        public void Remove()
        {
            BoardManager.Instance.PutOnBoard(TetrominoBlocks);
            TetrominoBlocks.Clear();
        }
        
        public BlockScript ChooseRandomBlock() => 
            TetrominoBlocks.Count > 0 ? TetrominoBlocks[Random.Range(0, TetrominoBlocks.Count)] : null;
    }
}