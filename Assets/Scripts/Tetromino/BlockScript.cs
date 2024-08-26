using Management.Pooling;
using UnityEngine;

namespace Tetromino
{
    public class BlockScript : MonoBehaviour
    {
        private void OnDisable()
        {
            PoolStore.Instance.Release(this);
        }
    }
}