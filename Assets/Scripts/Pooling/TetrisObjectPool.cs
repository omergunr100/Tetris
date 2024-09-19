using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pooling
{
    public class TetrisObjectPool<T> : IObjectPool<T> where T : MonoBehaviour
    {
        private readonly GameObject _poolObject;
        private readonly List<T> _pool = new();
        
        private readonly Func<T> _createObject;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onDestroy;
        private readonly int _maxSize;
        
        private int _currSize = 0;
        
        public TetrisObjectPool(Func<T> createObject,
            Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDestroy = null, 
            int maxSize = 500, int initialSize = 0)
        {
            _poolObject = new GameObject($"Pool {typeof(T)}");
            _createObject = createObject;
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
            _maxSize = maxSize;

            if (initialSize > 0)
            {
                var arr = new T[initialSize];
                
                for (var i = 0; i < initialSize; i++)
                    arr[i] = Get();

                for (var i = 0; i < initialSize; i++)
                    Release(arr[i]);
            }
        }
        
        public T Get()
        {
            T obj;
            
            if (_pool.Count > 0)
            {
                obj = _pool[0];
                obj.transform.SetParent(null);
                _pool.RemoveAt(0);
                _currSize--;
            }
            else
            {
                obj = _createObject();
            }
            _onGet?.Invoke(obj);

            return obj;
        }

        public void Release(T obj)
        {
            if (_pool.Contains(obj))
            {
                throw new ArgumentException("Tried to re-release a pooled object!");    
            }
            _onRelease?.Invoke(obj);
            if (_maxSize >= _currSize + 1)
            {
                obj.transform.SetParent(_poolObject.transform);
                _pool.Add(obj);
                _currSize++;
            }
            else
            {
                _onDestroy(obj);
                Object.Destroy(obj);
            }
        }

        public void Clear()
        {
            foreach (var behaviour in _pool)
            {
                _onDestroy?.Invoke(behaviour);
                Object.Destroy(behaviour);
            }
            _pool.Clear();

            _currSize = 0;
        }
    }
}