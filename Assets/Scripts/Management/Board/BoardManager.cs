using System.Collections.Generic;
using Management.Pooling;
using Management.Score;
using Tetromino;
using UnityEngine;
using Utils;

namespace Management.Board
{
    public class BoardManager : Singleton<BoardManager>
    {
        [SerializeField] private Camera mainCamera;
        
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        
        private BlockScript[,] _board;
        private GameObject _wallParent;
        
        public Vector3 SpawnPosition => new(width / 2f, height, 0);

        private void CreateBoard()
        {
            _board = new BlockScript[width, height + 1];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                _board[x, y] = null;
        }

        private void CreateBorder()
        {
            if (_wallParent == null)
                _wallParent = new GameObject("WallParent");
            
            for (var x = -1; x <= width; x++)
            {
                for (var y = -1; y <= height; y++)
                {
                    if (x == -1 || x == width || y == -1)
                    {
                        var wall = PoolStore.Instance.Get<BlockScript>();

                        wall.transform.position = new Vector3(x, y, 0);
                        var spriteRenderer = wall.GetComponentInParent<SpriteRenderer>();
                        spriteRenderer.color = Color.gray;
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
            Reset();
        }

        private (int, int) PositionToBoard(Vector3 p) => (Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y));

        private bool IsEmptyIndex(int x, int y)
        {
            if (x >= width || x < 0 || y < 0 || y > height) return false;
            return _board[x, y] == null;
        }
        
        public bool TryMove(Vector3 direction)
        {
            var mover = TetrominoScript.Instance;
            if (mover == null || !mover.gameObject.activeSelf) return false;
            
            var legal = true;
            foreach (var block in mover.TetrominoBlocks)
            {
                var newPos = block.transform.position + direction;
                var (x, y) = PositionToBoard(newPos);
                legal &= IsEmptyIndex(x, y);
            }

            if (legal) Move(mover.transform, direction);
            else if (ShouldStop()) mover.Remove();
            
            return legal;
        }

        public bool TryRotate()
        {
            var legal = true;
            var rotator = TetrominoScript.Instance;
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

        public bool ShouldStop()
        {
            var stop = false;
            
            foreach (var block in TetrominoScript.Instance.TetrominoBlocks)
            {
                var (x, y) = PositionToBoard(block.transform.parent.position + block.transform.localPosition);
                stop |= !IsEmptyIndex(x, y - 1);
            }
            
            return stop;
        }

        public void PutOnBoard(List<BlockScript> blocks)
        {
            var scoreBonus = 0;
            var fullRows = new HashSet<int>();
            
            foreach (var block in blocks)
            {
                // get the board position of the object
                var (x, y) = PositionToBoard(block.transform.position);
                
                // check for loss condition
                if (y == height)
                {
                    GameManager.Instance.OnLoss();
                    return;
                }
                
                // put the object on the board
                if (IsEmptyIndex(x, y))
                {
                    _board[x, y] = block;
                }
                
                // check if line is now full
                if (IsRowFull(y))
                {
                    Debug.Log($"Detected full row at {y}");
                    scoreBonus += 1;
                    fullRows.Add(y);
                }
            }
            
            ClearRows(fullRows);
            
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

        private void ClearRows(ISet<int> yList)
        {
            var emptySoFar = 0;
            for (var y = 0; y < height; y++)
            {
                if (yList.Contains(y))
                {
                    emptySoFar++;
                    for (var x = 0; x < width; x++)
                    {
                        var block = _board[x, y];
                        PoolStore.Instance.Release(block);
                        
                        _board[x, y] = null;
                    }
                }
                else if (emptySoFar > 0)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var obj = _board[x, y];
                        if (obj != null)
                        {
                            obj.transform.position += Vector3.down * emptySoFar;
                            _board[x, y - emptySoFar] = obj;
                            _board[x, y] = null;
                        }
                    }
                }
            }
        }

        public bool IsRowFull(int y)
        {
            var full = true;

            for (var x = 0; x < width && full; x++)
                full &= _board[x, y] != null;
            
            return full;
        }

        public void Reset()
        {
            CreateBoard();
            CreateBorder();
            DirectCamera();
        }

        public void Clear()
        {
            // todo: this is sus, might be another way to get all child transforms
            foreach (Transform blockTransform in _wallParent.transform)
            {
                blockTransform.transform.parent = null;
                PoolStore.Instance.Release(blockTransform.GetComponentInParent<BlockScript>());
            }
            
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var block = _board[x, y];
                if (block != null)
                {
                    PoolStore.Instance.Release(block);
                    _board[x, y] = null;
                }
            }
        }
    }
}