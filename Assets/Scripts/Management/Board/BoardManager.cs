using System.Collections.Generic;
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
            _board = new GameObject[width, height];
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
    }
}