using System;
using Assets.Prefabs.Characters.MainHero.Scripts.XPSystem;
using Cysharp.Threading.Tasks;
using Enemies.DmgInfliction;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Prefabs.ExpStar.Scripts
{
    public class ExpStar : MonoBehaviour
    {
        [SerializeField] private float _expAmount = 10f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _maxSpeed = 100f;
        [field:SerializeField] public Rigidbody rigibody { get; private set; }
        [SerializeField] private Collider _collider;
        [SerializeField] private LayerMask _player;
        [SerializeField] private float _pushPower = 5f;
        
        private ExpSystem _expSystem;
        private Transform _target;
        private IDisposable _dispose;

        private bool _isTouch;
        private Vector3 position => transform.position;
        private float distance => _target.IsUnityNull() ? 99.9f : Vector3.Distance(position, _target.position);

        public event Action<ExpStar> OnReachTarget = delegate { };

        private void Awake()
        {
            _expSystem = ExpSystem.Instance;
        }
        
        private void OnTriggerEnter(Collider other) {
            if (!_target.IsUnityNull() || other.gameObject.layer != _player.ToLayer()) return;
            
            _target = other.transform;
            FollowTarget().Forget();
        }

        public void Initialize(float expQuantity) {
            if(rigibody.IsUnityNull()) rigibody = gameObject.AddComponent<Rigidbody>();
            
            _expAmount = expQuantity;
            var forceDir = new Vector3(Random.Range(-1f, 1f),
                1f,
                Random.Range(-1f, 1f));
            
            if (rigibody.IsUnityNull()) Debug.Log($"RIGIDBODY NULL", this);
            rigibody.AddForce(forceDir * _pushPower, ForceMode.VelocityChange);
        }

        private async UniTaskVoid FollowTarget() {
            if(!rigibody.IsUnityNull()) Destroy(rigibody);
            _collider.isTrigger = true;

            do {
                await UniTask.Yield(destroyCancellationToken);

                _moveSpeed = Mathf.Lerp(_moveSpeed,_maxSpeed, 0.01f);
            
                transform.position = Vector3.MoveTowards(
                    transform.position, _target.position,
                    _moveSpeed * Time.deltaTime);
            } while (distance > 0.5f);
            
            _expSystem.ReceiveExp(_expAmount);
            OnReachTarget.Invoke(this);
        }
    }
}
