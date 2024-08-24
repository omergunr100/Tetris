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

        public TetrominoScript CurrentTetromino { get; set; } = null;

        private void CreateBoard()
        {
            _board = new GameObject[width, height + 1];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                _board[x, y] = null;
        }

        private void CreateBorder()
        {
            if (_wallParent == null)
                _wallParent = new GameObject("WallParent");
            var wallCount = 0;
            for (var x = -1; x <= width; x++)
            {
                for (var y = -1; y <= height; y++)
                {
                    if (x == -1 || x == width || y == -1)
                    {
                        GameObject wall;
                        if (wallCount < _border.Count)
                        {
                            wall = _border[wallCount];
                            wall.SetActive(true);
                        }
                        else
                        {
                            wall = new GameObject("Wall");
                            _border.Add(wall);
                        }

                        wall.transform.position = new Vector3(x, y, 0);
                        var spriteRenderer = wall.AddComponent<SpriteRenderer>();
                        spriteRenderer.sprite = wallSprite;
                        spriteRenderer.color = Color.gray;
                        wall.transform.SetParent(_wallParent.transform);
                        wallCount++;
                    }
                }
            }

            for (var i = wallCount; i < _border.Count; i++)
            {
                Destroy(_border[i]);
                _border.RemoveAt(i);
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
            var fullRows = new HashSet<int>();
            
            foreach (var obj in objects)
            {
                // get the board position of the object
                var (x, y) = PositionToBoard(obj.transform.position);
                
                // check for loss condition
                if (y == height)
                {
                    GameManager.Instance.OnLoss();
                    return;
                }
                
                // put the object on the board
                if (IsEmptyIndex(x, y))
                {
                    _board[x, y] = obj;
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
                    Debug.Log($"Clearing row {y}");
                    emptySoFar++;
                    for (var x = 0; x < width; x++)
                    {
                        Destroy(_board[x, y]);
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
            // destroy all block scripts
            foreach (var blockScript in FindObjectsByType<BlockScript>(FindObjectsSortMode.None))
                Destroy(blockScript.gameObject);
            
            // hide all walls
            foreach (var wall in _border)
            {
                wall.SetActive(false);
                wall.transform.parent = null;
            }
        }
    }
}