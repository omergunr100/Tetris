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

        public void Setup(TetrominoDefinition definition)
        {
            foreach (var directive in definition.blockDirectives)
            {
                var block = PoolStore.Instance.Get<BlockScript>();
                block.GetComponent<SpriteRenderer>().color = definition.pieceColor;
                TetrominoBlocks.Add(block);
                block.transform.parent = transform;
                block.transform.localPosition = directive.relativePosition;
                block.transform.localRotation = directive.rotation;
            }
        }

        public void Remove()
        {
            foreach (var block in TetrominoBlocks)
                block.transform.SetParent(null);
            BoardManager.Instance.PutOnBoard(TetrominoBlocks);
            TetrominoBlocks.Clear();
        }
    }
}