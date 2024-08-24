using UnityEngine;

namespace Tetromino
{
    public class BlockScript : MonoBehaviour
    {
        private void Awake()
        {
            GetComponentInParent<TetrominoScript>().RegisterBlock(gameObject);
        }
    }
}