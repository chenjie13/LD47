using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGDK.Core
{
    public class ComponentPool<T> : SimpleObjPool<T> where T : Component
    {
        public Transform PooledRoot { get; }
        public T Prefab { get; }
        
        public ComponentPool(T prefab, int capacity, Transform poolRoot = null, string poolPath = null)
        {
            Debug.Assert(prefab != null, "ComponentPool prefab == null");
            
            Prefab = prefab;
            Capacity = capacity;
            PooledRoot = poolRoot ? poolRoot.CreateChildren(poolPath) : new GameObject(prefab.name + "Pool").transform;

            _createFunc = () => Object.Instantiate(Prefab);
            _destroyFunc = obj => Object.Destroy(obj.gameObject);

            OnSpawn += obj => { obj.gameObject.SetActive(true); };
            OnRecycle += obj =>
            {
                obj.transform.SetParent(PooledRoot); 
                obj.gameObject.SetActive(false);
            };
        }
    }
}