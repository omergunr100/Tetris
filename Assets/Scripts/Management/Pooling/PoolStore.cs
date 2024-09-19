using System.Collections.Generic;
using Management.Prefabs;
using Pooling;
using Tetromino;
using UnityEngine;
using Utils;

namespace Management.Pooling
{
    public class PoolStore : Singleton<PoolStore>
    {
        private readonly Dictionary<string, object> _pools = new();

        private void Awake()
        {
            _pools.Add(typeof(BlockScript).ToString(), new TetrisObjectPool<BlockScript>(
                createObject: () => Instantiate(PrefabStore.Instance.blockPrefab),
                onGet: block => block.gameObject.SetActive(true),
                onRelease: block => block.gameObject.SetActive(false),
                initialSize: 100
            ));
        }

        public T Get<T>() where T : MonoBehaviour => (_pools[typeof(T).ToString()] as IObjectPool<T>)?.Get();

        public void Release<T>(T obj) where T : MonoBehaviour
        {
            if (_pools.TryGetValue(typeof(T).ToString(), out var pool))
            {
                ((IObjectPool<T>)pool).Release(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
        
        public void Clear<T>() where T : MonoBehaviour => (_pools[typeof(T).ToString()] as IObjectPool<T>)?.Clear();
    }
}