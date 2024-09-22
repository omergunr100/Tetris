using System.Collections.Generic;
using Management.Audio;
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
        
        [SerializeField] public int width = 10;
        [SerializeField] public int height = 20;

        public int Bottom { get; private set; } = -1;
        public int Left { get; private set; } = -1;
        
        private BlockScript[,] _board;
        private GameObject _wallParent;
        private GameObject _boardParent;

        private long _runningCount = 0;
        
        public Vector3 SpawnPosition => new(width / 2f, height, 0);

        private void CreateBoard()
        {
            if (_boardParent == null)
                _boardParent = new GameObject("Board");
            else 
                _boardParent.SetActive(true);
            _board = new BlockScript[width, height + 1];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                _board[x, y] = null;
        }

        private void CreateBorder()
        {
            if (_wallParent == null)
                _wallParent = new GameObject("WallParent");
            
            for (var x = Left; x <= width; x++)
            {
                for (var y = Bottom; y <= height; y++)
                {
                    if (x == Left || x == width || y == Bottom)
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
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
        }

        private void Start()
        {
            CreateBorder();
        }

        private void GamePhaseListener(GamePhase phase)
        {
            if (_wallParent != null)
                _wallParent.SetActive(phase == GamePhase.Game);
            if (_boardParent != null)
                _boardParent.SetActive(phase == GamePhase.Game);
            switch (phase)
            {
                case GamePhase.Game:
                    Reset();
                    break;
                case GamePhase.Loss:
                    Clear();
                    break;
            }
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
            
            for (var i = 0; i < blocks.Count; i++)
            {
                // get the board position of the object
                var (x, y) = PositionToBoard(blocks[i].transform.position);
                
                // check for loss condition
                if (y >= height)
                {
                    for (var j = i; j < blocks.Count; j++)
                        PoolStore.Instance.Release(blocks[j]);
                    GameManager.Instance.SetGamePhase(GamePhase.Loss);
                    return;
                }
                
                // put the object on the board
                if (IsEmptyIndex(x, y))
                {
                    _board[x, y] = blocks[i];
                    blocks[i].transform.SetParent(_boardParent.transform);
                    _board[x, y].IsOnBoard = true;
                    _board[x, y].BoardLocation = (x, y);
                    _board[x, y].Id = _runningCount;
                }
                
                // check if line is now full
                if (IsRowFull(y))
                {
                    Debug.Log($"Detected full row at {y}");
                    scoreBonus += 1;
                    fullRows.Add(y);
                }
            }
            
            if (fullRows.Count == 0)
                SoundManager.Instance.TetrominoLand();
            else
            {
                SoundManager.Instance.RowClearing();
                ClearRows(fullRows);
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
            _runningCount++;
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
                        _board[x, y].gameObject.name = "Block";
                        PoolStore.Instance.Release(_board[x, y]);
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

        private bool IsRowFull(int y)
        {
            var full = true;

            for (var x = 0; x < width && full; x++)
                full &= _board[x, y] != null;
            
            return full;
        }

        public void Reset()
        {
            CreateBoard();
            DirectCamera();
            _wallParent.SetActive(true);
        }

        private void Clear()
        {
            _wallParent.SetActive(false);
            
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                if (_board[x, y] != null)
                {
                    _board[x, y].gameObject.name = "Block";
                    PoolStore.Instance.Release(_board[x, y]);
                    _board[x, y] = null;
                }
            Debug.Log($"BoardParent status: {(_boardParent.transform.childCount == 0 ? "Ok" : "Very very bad")}");
        }
        
        public Vector3 GetUpcomingPosition(TetrominoDefinition definition)
        {
            var left = float.MaxValue;
            var top = float.MinValue;
            definition.blockDirectives.ForEach(directive =>
            {
                if (directive.relativePosition.x < left) left = directive.relativePosition.x;
                if (directive.relativePosition.y > top) top = directive.relativePosition.y;
            });
            return new Vector3(width + 2.5f - left, height - 0.5f - top);
        }
        
        public BlockScript GetBoardPosition(int x, int y) {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return _board[x, y];
            return null;
        }
        
        public void SetBoardPosition(int x, int y, BlockScript block)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                _board[x, y] = block;
        }

        public bool ReleaseBoardPosition(int x, int y)
        {
            if (x >= width || x < 0 || y >= height || y < 0 || _board[x, y] == null) return false;
            PoolStore.Instance.Release(_board[x, y]);
            _board[x, y] = null;
            return true;
        }
    }
}