using System.Collections;
using System.Collections.Generic;
using Management.Board;
using Management.Prefabs;
using Tetromino;
using Utils;
using Random = UnityEngine.Random;

namespace Management.Tetromino
{
    public class TetrominoSpawner : Singleton<TetrominoSpawner>
    {
        private readonly List<TetrominoScript> _tetrominos = new();
        private readonly Queue<TetrominoScript> _tetrominoQueue = new();
        private bool _isSpawning = false;
        
        private void Awake()
        {
            for (var i = 0; i < 2; i++)
                foreach (var prefab in PrefabStore.Instance.tetrominoPrefabs)
                    _tetrominos.Add(Instantiate(prefab));
        }

        public void StartSpawn()
        {
            if (!_isSpawning) StartCoroutine(DelayedSpawn());
        }
        
        private void Spawn()
        {
            if (_tetrominoQueue.Count == 0)
            {
                // add all tetrominos to the bucket in a random order
                var tetrominos = new List<TetrominoScript>(_tetrominos);
                while (tetrominos.Count > 0)
                {
                    var randomIndex = Random.Range(0, tetrominos.Count);
                    _tetrominoQueue.Enqueue(tetrominos[randomIndex]);
                    tetrominos.RemoveAt(randomIndex);
                }
            }
            
            var tetromino = _tetrominoQueue.Dequeue();
            tetromino.transform.position = BoardManager.Instance.SpawnPosition;
            tetromino.Setup();
            BoardManager.Instance.CurrentTetromino = tetromino;
        }

        public void Clear()
        {
            _tetrominoQueue.Clear();
        }

        private IEnumerator DelayedSpawn()
        {
            _isSpawning = true;
            yield return null;
            Spawn();
            _isSpawning = false;
        }
     }
}