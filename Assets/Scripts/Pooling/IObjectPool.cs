using UnityEngine;

namespace Pooling
{
    public interface IObjectPool<T> where T : MonoBehaviour
    {
        T Get();
        void Release(T obj);
        void Clear();
    }
}