using System;
using _Shoot_Kill.Prefabs.Traps.Scripts;
using Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace _Shoot_Kill.Prefabs.Traps.Spike.Scripts
{
    public class Spike : Trap, IDamageSource
    {
       [field: SerializeField] public float damage { get; private set; }
       
        public GameObject damageDealer { get; }
        
        [SerializeField] private float _delayBeforeTrigger = 0.6f;
        [SerializeField] private Image _image;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _triggerColor;

        private BoxCollider _collider;
        private float startTime;
        
        protected static readonly int isReload = Animator.StringToHash("isReload");

        protected override void Awake()
        {
            base.Awake();

            _collider = GetComponent<BoxCollider>();
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player") return;
            
            UseTrap().Forget();
        }
        
        private async UniTaskVoid UseTrap()
        {
            _collider.enabled = false;
            
            var time = 0f;
            
            while (time <= _delayBeforeTrigger)
            {
                time += Time.deltaTime;

                _image.color = Color.Lerp(_defaultColor, _triggerColor, time / _delayBeforeTrigger);

                await UniTask.Yield();
            }

            var colliders = Physics.OverlapBox(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Vector3.one  );

            foreach (var target in colliders)
            {
                if (target.TryGetComponent(out IDamageable damageable))
                    damageable.DealDamage(this);
            }

            
            _image.color = _defaultColor;
            animator.SetTrigger(isTouch);

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            
            animator.SetTrigger(isReload);
        }

        private void ActivateCollider() => _collider.enabled = true;
    }
}