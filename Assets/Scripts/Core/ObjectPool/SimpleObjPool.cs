using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGDK.Core
{
    /// <summary>
    /// 简易对象池
    /// </summary>
    public class SimpleObjPool<T> : IPool<T>
    {
        private int _capacity;
        public int Capacity
        {
            get { return _capacity; }
            set
            {
                if (value <= 0) 
                    throw new ArgumentException("Capacity cant <= 0");
                _capacity = value;
            }
        }

        public int PooledCount
        {
            get { return _pooledObjs.Count; }
        }

        public bool HasPooled
        {
            get { return PooledCount > 0; }
        }

        public event Action<T> OnCreate; 
        public event Action<T> OnSpawn;
        public event Action<T> OnRecycle;
        public event Action<T> OnDestroy;

        protected Action<T> _destroyFunc;
        protected Func<T> _createFunc;
        protected Queue<T> _pooledObjs = new Queue<T>();

        public SimpleObjPool(Func<T> createFunc, Action<T> destroyFunc, int capacity)
        {
            Debug.Assert(createFunc != null, "createFunc == null");
            Debug.Assert(destroyFunc != null, "destroyFunc == null");
            
            _createFunc = createFunc;
            _destroyFunc = destroyFunc;
            Capacity = capacity;
        }
        
        protected SimpleObjPool() {}

        public virtual T Spawn()
        {
            var result = HasPooled ? _pooledObjs.Dequeue() : CreateNew();
            OnSpawn?.Invoke(result);
            return result;
        }

        public virtual void Recycle(T obj)
        {
            if (_pooledObjs.Count < Capacity)
            {
                _pooledObjs.Enqueue(obj);
                OnRecycle?.Invoke(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        public void Clear()
        {
            for (var i = 0; i < _pooledObjs.Count; i++)
            {
                Destroy(_pooledObjs.Dequeue());
            }
        }

        protected virtual T CreateNew()
        {
            var result = _createFunc.Invoke();
            OnCreate?.Invoke(result);
            return result;
        }    

        protected virtual void Destroy(T obj)
        {
            OnDestroy?.Invoke(obj);
            _destroyFunc.Invoke(obj);
        }

        public void Dispose()
        {
            Clear();
        }
    }
}