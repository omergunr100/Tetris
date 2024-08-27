using System.Collections.Generic;
using Management.Board;
using Management.Pooling;
using UnityEngine;

namespace Tetromino
{
    public class TetrominoScript : MonoBehaviour
    {
        // just in case we choose to implement as a list of directives and tetromino singleton
        [SerializeField] public readonly List<List<BlockDirective>> Directives = new();
        
        [SerializeField] private Color color;
        [SerializeField] private List<BlockDirective> blockDirectives;
            
        public readonly List<BlockScript> TetrominoBlocks = new();

        public void Setup()
        {
            gameObject.SetActive(true);
            foreach (var directive in blockDirectives)
            {
                var block = PoolStore.Instance.Get<BlockScript>();
                block.GetComponent<SpriteRenderer>().color = color;
                TetrominoBlocks.Add(block);
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
            foreach (var block in TetrominoBlocks)
            {
                block.transform.SetParent(null);
            }
            BoardManager.Instance.PutOnBoard(TetrominoBlocks);
            gameObject.SetActive(false);
        }
    }
}