using System;
using Characters;
using NaughtyAttributes;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies.DmgInfliction
{
    public abstract class DamageInfliction : MonoBehaviour
    {
        [Header("Base Dependencies")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField, ReadOnly] protected Transform target;
        
        [Header("Base Parameters")]
        [SerializeField, ReadOnly] protected float _damage;
        [SerializeField] protected float _attackDistance;
        [SerializeField] protected LayerMask obstructions;
        [SerializeField] protected LayerMask targetMask;

        protected static readonly int AttackTrigger = Animator.StringToHash("Attack");
        protected Vector3 position {
            get => transform.position;
            set => transform.position = value;
        }
        protected Quaternion rotation => transform.rotation;
        protected float attackDistance => _attackDistance;
        protected EnemyDamageDealer damageDeal;
        private IDisposable _disposable;

        private void Start() {
            CreateStackTrace();
        }

        private void OnValidate() {
            InitComponents();
        }

        /// <summary>
        /// Вызов реализации атаки.
        /// Этот метод должен вызываться сугубо из анимации.
        /// </summary>
        public abstract void Assault();

        public virtual DamageInfliction Initialize(float dmg) {
            _damage = dmg;
            damageDeal = new EnemyDamageDealer(_damage, gameObject);

            return this;
        }

        public void SetTarget(Transform target) => this.target = target;

        private void CreateStackTrace() {
            Observable.EveryUpdate(destroyCancellationToken)
                .Where(_ => agent.hasPath && agent.remainingDistance > 0)
                .Subscribe(_ => {
                    var isAttack = agent.remainingDistance < _attackDistance && TargetInSight();
                    animator.SetBool(AttackTrigger, isAttack);
                });
        }

        private bool TargetInSight() {
            var dir = target.position - position;
            var ray = new Ray(position, dir);
            return !Physics.Raycast(ray, agent.remainingDistance, obstructions);
        }

        protected virtual void InitComponents() {
            if (agent.IsUnityNull()) GetComponent<Collider>().transform.root.TryGetComponent(out agent);
            if (animator.IsUnityNull()) TryGetComponent(out animator);
        }
    }
}