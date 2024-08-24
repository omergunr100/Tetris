using System.Collections.Generic;
using Management;
using UnityEngine;

namespace Tetromino
{
    public class TetrominoScript : MonoBehaviour
    {
        [SerializeField] private Color color;
        
        private readonly List<GameObject> _tetrominoBlocks = new();

        public void RegisterBlock(GameObject block) => _tetrominoBlocks.Add(block);

        private void Start()
        {
            foreach (var tetrominoBlock in _tetrominoBlocks)
            {
                tetrominoBlock.GetComponent<SpriteRenderer>().color = color;
            }
            GameManager.Instance.CurrentTetrominos.Add(this);
        }

        public void Move(Vector3 offset)
        {
            transform.position += offset;
        }

        public void Rotate()
        {
            // rotate all blocks around the center
            var center = transform.position;
            foreach (var tetrominoBlock in _tetrominoBlocks)
            {
                var block = tetrominoBlock.transform;
                var offset = block.position - center;
                var rotatedOffset = new Vector3(offset.y, -offset.x, center.z);
                block.position = center + rotatedOffset;
            }
        }

        private void OnDestroy()
        {
            foreach (var block in _tetrominoBlocks)
            {
                block.transform.SetParent(null);
            }
            GameManager.Instance.CurrentTetrominos.Remove(this);
        }
    }
}