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
        private readonly Queue<TetrominoDefinition> _tetrominoQueue = new();
        
        public void Spawn()
        {
            if (_tetrominoQueue.Count == 0)
            {
                // add all tetrominos to the bucket in a random order
                var tetrominos = new List<TetrominoDefinition>(PrefabStore.Instance.tetrominoDataPrefabs);
                tetrominos.AddRange(PrefabStore.Instance.tetrominoDataPrefabs);
                while (tetrominos.Count > 0)
                {
                    var randomIndex = Random.Range(0, tetrominos.Count);
                    _tetrominoQueue.Enqueue(tetrominos[randomIndex]);
                    tetrominos.RemoveAt(randomIndex);
                }
            }
            
            var definition = _tetrominoQueue.Dequeue();
            TetrominoScript.Instance.transform.position = BoardManager.Instance.SpawnPosition;
            TetrominoScript.Instance.Setup(definition);
        }

        public void Clear()
        {
            _tetrominoQueue.Clear();
        }
     }
}