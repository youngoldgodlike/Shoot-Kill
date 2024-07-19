using System;
using UnityEngine;

namespace Architecture.MinePool
{
    public class DisposablePool<T> : Pool<T>
    {
        public event Action OnDispose = delegate { };
        
        public DisposablePool(Func<T> create, Action<T> @return, Action<T> get, int capacity) : base(create, @return, get, capacity) {
        }

        protected override T OnEmptyQueue() {
            Debug.LogWarning("DISPOSABLE POOL DISPOSING");
            OnDispose.Invoke();
            return default;
        }
    }
}