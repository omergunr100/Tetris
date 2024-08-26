using System;
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
        private readonly IObjectPool<BlockScript> _blockPool = new TetrisObjectPool<BlockScript>(
            createObject: () => Instantiate(PrefabStore.Instance.blockPrefab),
            onGet: block => block.gameObject.SetActive(true), 
            initialSize: 100
        );

        private readonly Dictionary<Type, object> _pools = new();

        private void Awake()
        {
            _pools.Add(typeof(BlockScript), _blockPool);
        }

        public T Get<T>() where T : MonoBehaviour => (_pools[typeof(T)] as IObjectPool<T>)?.Get();

        public void Release<T>(T obj) where T : MonoBehaviour
        {
            if (_pools.TryGetValue(typeof(T), out var pool))
                ((IObjectPool<T>) pool).Release(obj);
            else
                Destroy(obj);
        }
        
        public void Clear<T>() where T : MonoBehaviour => (_pools[typeof(T)] as IObjectPool<T>)?.Clear();
    }
}