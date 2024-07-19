using System;
using UnityEngine;

namespace _Shoot_Kill.Architecture.Scripts.EnemySpawn
{
    public class ExpandablePool<T> : SimplePool<T>
    {
        protected ExpandablePool(Action<T> getAction, Action<T> returnAction, Func<T> createAction, int capacity)
            : base(getAction, returnAction, createAction, capacity) {
        }

        protected override T EmptyPool() {
            return _createAction();
        }
    }
}

namespace Architecture.MinePool
{
    public class ExpandablePool<T> : Pool<T>
    {
        public ExpandablePool(Func<T> create, Action<T> @return, Action<T> get, int capacity) : base(create, @return, get, capacity) {
        }

        protected override T OnEmptyQueue() {
            var newItem = create();
            
            activeItems.Add(newItem);
            get(newItem);
            
            return newItem;
        }
    }
}