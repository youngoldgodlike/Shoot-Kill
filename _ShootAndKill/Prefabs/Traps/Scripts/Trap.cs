using UnityEngine;

namespace _Shoot_Kill.Prefabs.Traps.Scripts
{
    public abstract class Trap : MonoBehaviour
    {
        protected Animator animator;

        protected static readonly int isTouch = Animator.StringToHash("isTouch");
    
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected abstract void OnTriggerEnter(Collider other);
        
        public void DestroyTrap() => Destroy(gameObject);
    }
}