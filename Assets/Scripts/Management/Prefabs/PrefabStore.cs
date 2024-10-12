using Tetromino;
using UnityEngine;
using Utils;

namespace Management.Prefabs
{
    public class PrefabStore : Singleton<PrefabStore>
    {
        [SerializeField] public BlockScript blockPrefab; 
        [SerializeField] public TetrominoDefinition[] tetrominoDataPrefabs;

        [SerializeField] public GameObject stickFigure;
    }
}