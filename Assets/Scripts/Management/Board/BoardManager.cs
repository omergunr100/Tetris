using System.Collections.Generic;
using Management.Score;
using Tetromino;
using UnityEngine;
using Utils;

namespace Management.Board
{
    public class BoardManager : Singleton<BoardManager>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Sprite wallSprite;
        
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        
        private GameObject[,] _board;
        private readonly List<GameObject> _border = new();
        private GameObject _wallParent;
        
        public Vector3 SpawnPosition => new(width / 2f, height, 0);

        private void CreateBoard()
        {
            _board = new GameObject[width, height + 1];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                _board[x, y] = null;
        }

        private void CreateBorder()
        {
            _wallParent = new GameObject("WallParent");
            for (var x = -1; x <= width; x++)
            {
                for (var y = -1; y <= height; y++)
                {
                    if (x == -1 || x == width || y == -1)
                    {
                        var wall = new GameObject("Wall");
                        wall.transform.position = new Vector3(x, y, 0);
                        var spriteRenderer = wall.AddComponent<SpriteRenderer>();
                        spriteRenderer.sprite = wallSprite;
                        spriteRenderer.color = Color.gray;
                        _border.Add(wall);
                        wall.transform.SetParent(_wallParent.transform);
                    }
                }
            }
            _wallParent.transform.position += Vector3.right * 0.5f + Vector3.up * 0.5f;
        }

        private void DirectCamera()
        {
            var cameraPosition = new Vector3(width / 2f, height / 2f, -10);
            mainCamera.transform.position = cameraPosition;
            mainCamera.orthographicSize = height / 1.5f;
        }
        
        private void Awake()
        {
            CreateBoard();
            CreateBorder();
            DirectCamera();
        }

        public (int, int) PositionToBoard(Vector3 p) => (Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y));

        public bool IsEmptyIndex(int x, int y)
        {
            if (x >= width || x < 0 || y < 0 || y > height) return false;
            return _board[x, y] == null;
        }
        
        public bool TryMove(TetrominoScript mover, Vector3 direction)
        {
            var legal = true;
            foreach (var block in mover.TetrominoBlocks)
            {
                var newPos = block.transform.position + direction;
                var (x, y) = PositionToBoard(newPos);
                legal &= IsEmptyIndex(x, y);
            }

            if (legal) Move(mover.transform, direction);
            
            return legal;
        }

        public bool TryRotate(TetrominoScript rotator)
        {
            var legal = true;
            
            var center = rotator.transform.position;
            var length = rotator.TetrominoBlocks.Count;
            var newPositions = new Vector3[length];
            for (var i = 0; i < length; i++)
            {
                var block = rotator.TetrominoBlocks[i];
                var tr = block.transform;
                var offset = tr.position - center;
                var rotatedOffset = new Vector3(offset.y, -offset.x, center.z);
                newPositions[i] = center + rotatedOffset;
                var (x, y) = PositionToBoard(newPositions[i]);
                legal &= IsEmptyIndex(x, y);
            }
            
            for (var i = 0; legal && i < length; i++)
                rotator.TetrominoBlocks[i].transform.position = newPositions[i];
            
            return legal;
        }

        public void Move(Transform t, Vector3 offset) => t.position += offset;

        public bool ShouldStop(TetrominoScript tetromino)
        {
            var stop = false;

            foreach (var block in tetromino.TetrominoBlocks)
            {
                var (x, y) = PositionToBoard(block.transform.position);
                stop |= !IsEmptyIndex(x, y - 1);
            }
            
            return stop;
        }

        public void PutOnBoard(GameObject[] objects)
        {
            var scoreBonus = 0;
            
            foreach (var obj in objects)
            {
                // get the board position of the object
                var (x, y) = PositionToBoard(obj.transform.position);
                
                // check for loss condition
                if (y == height)
                {
                    // todo: we lost, do not add score and notify game manager
                    return;
                }
                
                // put the object on the board
                if (IsEmptyIndex(x, y))
                    _board[x, y] = obj;
                
                // check if line is now full
                if (IsRowFull(y))
                {
                    scoreBonus += 1;
                    ClearRow(y);
                }
            }
            
            // add score to current score
            var currentLineBonus = 100;
            var totalAdd = 0;
            for (var i = 0; i < scoreBonus; i++)
            {
                totalAdd += currentLineBonus;
                currentLineBonus += 100;
            }
            ScoreManager.Instance.AddScore(totalAdd);
        }

        private void ClearRow(int y)
        {
            // todo: clear the row, remove all blocks and drop all rows above one down
        }

        public bool IsRowFull(int y)
        {
            var full = true;

            for (var x = 0; x < width; x++)
                full &= _board[x, y] != null;
            
            return full;
        }
    }
}