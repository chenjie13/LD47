
using System;

namespace CGDK.Core
{
    public interface IPool<T> : IDisposable
    {
        int Capacity { get; set; }
        
        int PooledCount { get; }
        
        bool HasPooled { get; }
        
        T Spawn();

        void Recycle(T obj);
    }
}