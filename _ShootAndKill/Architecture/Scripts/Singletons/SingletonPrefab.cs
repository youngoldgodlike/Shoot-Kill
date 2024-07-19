using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Helpers.Debugging
{
    public abstract class SingletonPrefab<T> : MonoBehaviour where T : Component
    {
        protected static T _prefab;
        
        protected static T instance;

        public static T Instance => instance.IsUnityNull() ? CreateSingleton() as T : instance;

        private void OnValidate() {
            Bootstrap();
        }

        protected virtual void Awake() {
            if(_prefab.IsUnityNull()) Bootstrap();
            if (!instance.IsUnityNull()) Destroy(gameObject);
            instance = this as T;
            DontDestroyOnLoad(this);
        }

        protected abstract void Bootstrap();
        

        private static async UniTask<T> CreateSingleton() {
            await UniTask.WaitUntil(() => !_prefab.IsUnityNull());
            instance = Instantiate(_prefab);
            return instance;
        }
    }
}