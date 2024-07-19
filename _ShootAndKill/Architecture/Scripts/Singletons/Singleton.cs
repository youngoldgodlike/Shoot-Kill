using Unity.VisualScripting;
using UnityEngine;

namespace _Shoot_Kill.Architecture.Scripts
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;
        
        public static T Instance {
            get {
                if (instance.IsUnityNull()) {
                    instance = FindObjectOfType<T>();
                    if (instance.IsUnityNull()) {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }
                
                return instance;
            }
        }

        public static bool hasInstance => Instance != null;
        public static T TryGetInstance() => hasInstance ? Instance : null;

        protected virtual void Awake() {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton() {
            if(!Application.isPlaying) return;

            instance = this as T;
        }
    }
}
