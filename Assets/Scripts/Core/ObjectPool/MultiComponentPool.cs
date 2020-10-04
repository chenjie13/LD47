using System.Collections.Generic;
using UnityEngine;

namespace CGDK.Core
{
    public class MultiComponentPool<T> where T : Component
    {
        public Transform PoolRoot { get; protected set; }
        
        protected Dictionary<string, IPool<T>> _poolDict = new Dictionary<string, IPool<T>>();
        protected static int _nextGoId;

        public MultiComponentPool(string name)
        {
            PoolRoot = new GameObject(name).transform;
        }

        public IPool<T> this[string key]
        {
            get
            {
                _poolDict.TryGetValue(key, out var result);
                return result;
            }
        }

        public string AddNewPool(T prefab)
        {
            var key = GetKeyByPrefab(prefab);
            if (ExistKey(key))
            {
                LogUtil.Error("Already register");
            }
            else
            {
                var root = new GameObject(key).transform;
                root.SetParent(PoolRoot);
                var pool = new ComponentPool<T>(prefab, 30, root);
                _poolDict.Add(key, pool);
                pool.OnCreate += obj => { obj.gameObject.name = key + "_" + _nextGoId++; };
            }

            return key;
        }

        public bool ExistKey(string key)
        {
            return _poolDict.ContainsKey(key);
        }

        public bool TryRecycle(T obj)
        {
            var key = GetKeyByInstance(obj);

            if (string.IsNullOrEmpty(key) || !ExistKey(key))
            {
//                LogUtil.Warning("Cant find a pool to recycle " + key);
                return false;
            }

            _poolDict[key].Recycle(obj);
            return true;
        }

        public T Spawn(T prefab)
        {
            var key = GetKeyByPrefab(prefab);
            if (!ExistKey(key))
            {
                AddNewPool(prefab);
            }

            return _poolDict[key].Spawn();
        }

        public T Spawn(string key)
        {
            if (ExistKey(key)) return default(T);
            return _poolDict[key].Spawn();
        }

        public string GetKeyByInstance(T obj)
        {
            var index = obj.name.LastIndexOf('_');
            return index < 0 ? null : obj.name.Substring(0, index);
        }

        public string GetKeyByPrefab(T prefab)
        {
            return prefab.gameObject.name;
        }
    }
}