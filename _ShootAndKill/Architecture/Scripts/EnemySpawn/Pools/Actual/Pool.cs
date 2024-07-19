using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Architecture.MinePool
{
    [Serializable]
    public abstract class Pool<T>
    {
        protected Action<T> @return, get;
        protected Func<T> create;

        protected readonly Queue<T> items = new();
        protected readonly List<T> activeItems = new();
        protected int capacity;

        protected event Action<T> OnReturnItem = delegate {  };
        protected event Action<T> OnGetItem = delegate {  };
        protected event Action<T> OnCreateItem = delegate {  };
        
        public bool isEmpty => items.Count + activeItems.Count == 0;
        public float count => items.Count + activeItems.Count;

        protected Pool(Func<T> create, Action<T> @return, Action<T> get, int capacity) {
            this.create = create;
            this.@return = @return;
            this.get = get;
            this.capacity = capacity;
            
            InitializePool().Forget();
        }

        private async UniTaskVoid InitializePool() {
            await UniTask.Yield();
            
            for (var i = 0; i < capacity; i++) {
                Create();

                await UniTask.Yield();
            }
        }

        protected void Create() {
            var item = create();
            OnCreateItem.Invoke(item);
            Return(item);
        }

        public void Return(T item) {
            items.Enqueue(item);
            activeItems.Remove(item);
            OnReturnItem.Invoke(item);
            @return(item);
        }

        public T Get() {
            if (!items.TryDequeue(out var item)) return OnEmptyQueue();

            activeItems.Add(item);
            OnGetItem.Invoke(item);
            get(item);

            return item;
        }

        protected abstract T OnEmptyQueue();
    }
}