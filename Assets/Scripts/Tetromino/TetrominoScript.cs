using System.Collections.Generic;
using Management.Board;
using UnityEngine;

namespace Tetromino
{
    public class TetrominoScript : MonoBehaviour
    {
        [SerializeField] private Color color;

        public List<GameObject> TetrominoBlocks { get; } = new();

        public void RegisterBlock(GameObject block) => TetrominoBlocks.Add(block);

        private void Start()
        {
            foreach (var tetrominoBlock in TetrominoBlocks)
            {
                tetrominoBlock.GetComponent<SpriteRenderer>().color = color;
            }
            BoardManager.Instance.CurrentTetromino = this;
        }

        public void Move(Vector3 offset)
        {
            if (!BoardManager.Instance.TryMove(this, offset) && BoardManager.Instance.ShouldStop(this))
                Destroy(this);
        }

        public void Rotate()
        {
            BoardManager.Instance.TryRotate(this);
        }

        private void OnDestroy()
        {
            BoardManager.Instance.CurrentTetromino = null;
            foreach (var block in TetrominoBlocks)
            {
                block.transform.SetParent(null);
            }
            BoardManager.Instance.PutOnBoard(TetrominoBlocks.ToArray());
        }
    }
}