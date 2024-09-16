using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Management.Pooling;
using UnityEngine;

namespace Tetromino
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Tetromino Definition")]
    public class TetrominoDefinition : ScriptableObject
    {
        [SerializeField] public Color pieceColor;
        [SerializeField] public List<BlockDirective> blockDirectives;

        public static void Setup(TetrominoDefinition definition, ICollection<BlockScript> blocks, GameObject parent)
        {
            foreach (var directive in definition.blockDirectives)
            {
                var block = PoolStore.Instance.Get<BlockScript>();
                block.GetComponent<SpriteRenderer>().color = definition.pieceColor;
                blocks.Add(block);
                block.transform.parent = parent.transform;
                block.transform.localPosition = directive.relativePosition;
                block.transform.localRotation = directive.rotation;
            }
        }
    }
}