using UnityEngine;

namespace Assets.Architecture.Scripts.PoolObject
{
    public abstract class Pool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private int _poolCapacity = 3;
        [SerializeField] private bool _autoExpand = false;
        [SerializeField] private T _prefab;
        
        protected PoolMono<T> _pool;
        
        private void Start()
        {
            _pool = new PoolMono<T>(_prefab, _poolCapacity, transform);
            _pool.autoExpand = _autoExpand;
        }

        public virtual T GetObject(bool isActive)
        {
            var obj = _pool.TryGetFreeElement(isActive);
            return obj;
        }
        
    }
}
