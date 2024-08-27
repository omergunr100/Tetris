using System.Collections.Generic;
using UnityEngine;

namespace Tetromino
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Tetromino Definition")]
    public class TetrominoDefinition : ScriptableObject
    {
        [SerializeField] public Color pieceColor;
        [SerializeField] public List<BlockDirective> blockDirectives;
    }
}