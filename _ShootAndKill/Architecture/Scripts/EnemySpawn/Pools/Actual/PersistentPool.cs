using System;

namespace Architecture.MinePool
{
    public class PersistentPool<T> : Pool<T>
    {
        public PersistentPool(Func<T> create, Action<T> @return, Action<T> get, int capacity) : base(create, @return, get, capacity) {
        }

        protected override T OnEmptyQueue() {
            return default;
        }
    }
}