using System.Collections.Generic;
using Management.Board;
using UnityEngine;
using Utils;

namespace Management.Tetromino
{
    public class TetrominoSpawner : Singleton<TetrominoSpawner>
    {
        [SerializeField] private GameObject[] tetrominoPrefabs;

        private readonly Queue<GameObject> _tetrominoQueue = new();

        public void Spawn()
        {
            if (_tetrominoQueue.Count == 0)
            {
                // add all tetrominos to the bucket in a random order
                var tetrominos = new List<GameObject>(tetrominoPrefabs);
                tetrominos.AddRange(tetrominoPrefabs);
                while (tetrominos.Count > 0)
                {
                    var randomIndex = Random.Range(0, tetrominos.Count);
                    _tetrominoQueue.Enqueue(tetrominos[randomIndex]);
                    tetrominos.RemoveAt(randomIndex);
                }
            }
            
            var tetromino = Instantiate(_tetrominoQueue.Dequeue());
            tetromino.transform.position = BoardManager.Instance.SpawnPosition;
        }

        public void Clear()
        {
            _tetrominoQueue.Clear();
        }
    }
}