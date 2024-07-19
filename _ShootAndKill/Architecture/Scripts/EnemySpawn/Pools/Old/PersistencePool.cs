using System;

namespace _Shoot_Kill.Architecture.Scripts.EnemySpawn
{
    public class PersistencePool<T> : SimplePool<T>
    {
        protected PersistencePool(Action<T> getAction, Action<T> returnAction, Func<T> createAction, int capacity)
            : base(getAction, returnAction, createAction, capacity) {
        }
        
        protected override T EmptyPool() {
            return default;
        }
        
        public bool TryGetItem(out T item) {
            if (pool.Count > 0) {
                item = pool.Dequeue();
                activePool.Add(item);
                _getAction(item);
                return true;
            }
            item = default(T);
            return false;
        }
    }
}