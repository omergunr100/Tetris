using System.Collections.Generic;
using Management.Board;
using Management.Pooling;
using UnityEngine;

namespace Tetromino
{
    public class TetrominoScript : MonoBehaviour
    {
        [SerializeField] private Color color;
        [SerializeField] private List<BlockDirective> blockDirectives;
            
        private readonly List<BlockScript> _tetrominoBlocks = new();

        public void Setup()
        {
            foreach (var directive in blockDirectives)
            {
                var block = PoolStore.Instance.Get<BlockScript>();
                block.GetComponent<SpriteRenderer>().color = color;
                _tetrominoBlocks.Add(block);
                block.transform.parent = transform;
                block.transform.localPosition = directive.relativePosition;
                block.transform.localRotation = directive.rotation;
            }
            // todo: think about implementing tetromino script as singleton
            // todo: instead of prefabs have a list of lists of block directives
            BoardManager.Instance.CurrentTetromino = this;
        }

        public void Remove()
        {
            BoardManager.Instance.CurrentTetromino = null;
            foreach (var block in _tetrominoBlocks)
            {
                block.transform.SetParent(null);
            }
            BoardManager.Instance.PutOnBoard(_tetrominoBlocks);
        }

        public void Move(Vector3 offset)
        {
            if (!BoardManager.Instance.TryMove(this, offset) && BoardManager.Instance.ShouldStop(this))
                Remove();
        }

        public void Rotate()
        {
            BoardManager.Instance.TryRotate(this);
        }
    }
}