using UnityEngine;

namespace Tetromino
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Block Directive")]
    public class BlockDirective : ScriptableObject
    {
        [SerializeField] public Vector3 relativePosition;
        [SerializeField] public Quaternion rotation;
    }
}