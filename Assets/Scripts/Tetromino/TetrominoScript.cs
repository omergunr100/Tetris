using System.Collections.Generic;
using Management.Board;
using Management.Pooling;
using UnityEngine;
using Utils;

namespace Tetromino
{
    public class TetrominoScript : Singleton<TetrominoScript>
    {
        public readonly List<BlockScript> TetrominoBlocks = new();
        public bool Removed => TetrominoBlocks.Count == 0;

        public void Setup(TetrominoDefinition definition) => TetrominoDefinition.Setup(definition, TetrominoBlocks, gameObject);

        public void Remove()
        {
            foreach (var block in TetrominoBlocks)
                block.transform.SetParent(null);
            BoardManager.Instance.PutOnBoard(TetrominoBlocks);
            TetrominoBlocks.Clear();
        }
    }
}