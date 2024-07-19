using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Shoot_Kill.Architecture.Scripts.EnemySpawn
{
    public abstract class SimplePool<T>
    {
        protected readonly Queue<T> pool = new();
        protected readonly List<T> activePool = new();
        protected readonly Action<T> _getAction, _returnAction;
        protected readonly Func<T> _createAction;

        protected SimplePool(Action<T> getAction, Action<T> returnAction, Func<T> createAction, int capacity) {
            _getAction = getAction;
            _returnAction = returnAction;
            _createAction = createAction;

            for (int i = 0; i < capacity; i++) {
                ReturnInPool(_createAction());
            }
        }

        public void ReturnInPool(T item) {
            _returnAction(item);
            pool.Enqueue(item);
            activePool.Remove(item);
        }

        public T GetItem() {
            var item = pool.Count != 0 ? pool.Dequeue() : EmptyPool();
            _getAction(item);
            activePool.Add(item);

            return item;
        }

        protected abstract T EmptyPool();
    }
}