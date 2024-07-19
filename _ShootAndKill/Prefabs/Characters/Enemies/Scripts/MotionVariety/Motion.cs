using System;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;
using Enemies.EnemiesVariety;
using NaughtyAttributes;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemies.MotionVariety
{
    #region Traits
    
    public abstract class Trait{}
    public abstract class RotationTrait : Trait
    {}
    public interface ITrait<T> where T : Trait{}
    public interface IRotationTrait<T> where T : Trait{}
    public class LowRotation : RotationTrait{}
    public class DefaultRotation : RotationTrait{}
    
    public static class RotationTraits
    {
        public static Quaternion Rotate(this IRotationTrait<DefaultRotation> trait,Transform target,Transform me,float angularSpeed) {
            return target.rotation;
        }

        public static Quaternion Rotate(this IRotationTrait<LowRotation> trait, Transform target, Transform me,
            float angularSpeed) {
            return target.rotation;
        }
    }
    
    #endregion

    public abstract class Rotator
    {
        protected readonly Transform me;
        // protected Transform target;
        protected float angularSpeed;
        
        public Rotator(Transform me, float angularSpeed) {
            //this.target = target;
            this.me = me;
            this.angularSpeed = angularSpeed;
        }
        
        public abstract void Rotate();
        protected abstract Vector3 GetDestination();

        protected Vector3 DirectionToTarget() {
            return Vector3.ProjectOnPlane(GetDestination() - me.position, Vector3.up);
        }

        protected Quaternion ViewAtTarget() {
            var dir = DirectionToTarget();
            return dir != Vector3.zero ? Quaternion.LookRotation(DirectionToTarget()) : Quaternion.identity;
        }
    }

    public class LowRotator : Rotator
    {
        private readonly Transform _target;
        private const float MIN_ANGULAR_SPEED = 20f;

        public float currentAngleSpeed { get; private set; }

        public LowRotator(Transform target, Transform me, float angularSpeed) : base(me, angularSpeed) {
            _target = target;
        }

        public override void Rotate() {
            Debug.DrawRay(me.position, DirectionToTarget(), Color.green);
            var dot = Vector3.Angle(me.forward, DirectionToTarget()) / 180;
            var currentAngSpeed = Mathf.Lerp(angularSpeed, MIN_ANGULAR_SPEED, dot + 0.1f);
            currentAngleSpeed = currentAngSpeed;

            me.rotation = Quaternion.RotateTowards(me.rotation, ViewAtTarget(), currentAngSpeed * Time.deltaTime);
        }

        protected override Vector3 GetDestination() {
            return _target.position;
        }
    }

    public class DefaultRotator : Rotator
    {
        private readonly NavMeshAgent _agent;

        public DefaultRotator(NavMeshAgent agent, Transform me, float angularSpeed) : base(me, angularSpeed) {
            _agent = agent;
        }

        public override void Rotate() {
            var toQuaternion = ViewAtTarget();
            
            me.rotation = Quaternion.RotateTowards(me.rotation, toQuaternion, angularSpeed * Time.deltaTime);
        }

        protected override Vector3 GetDestination() {
            return _agent.nextPosition;
        }
    }

    public abstract class Motion : MonoBehaviour
    {
        [Header("Base Dependencies")] 
        [SerializeField] protected Health hp;
        [SerializeField] protected Enemy enemy;
        [SerializeField] protected Animator animator;
        [SerializeField] protected NavMeshAgent agent;

        [Header("Base Parameters")] 
        [SerializeField] protected float stopDistance;
        [SerializeField, ReadOnly] protected float motionSpeed;
        [SerializeField, ReadOnly] protected Transform target;
        [SerializeField] protected float angularSpeed = 120f;
        [SerializeField] protected MovementBehavior movement = MovementBehavior.Approach;
        [SerializeField] protected RotationBehavior rotate = RotationBehavior.Default;
        
        protected MotionBehavior motionBehavior;
        protected Rotator rotator;

        [ShowNativeProperty]
        private float currentAngle {
            get {
                if (rotator is LowRotator rot) return rot.currentAngleSpeed;
                return angularSpeed;
            }
        }

        [Header("Agent")]
        [SerializeField] protected bool updateRotation = false;
        [SerializeField] protected bool updatePosition;

        protected float velocity {
            get {
                if (agent.IsUnityNull()) return 0f;
                if (Mathf.Abs(agent.nextPosition.y - transform.position.y) > 0.5f) return 0f; // проверка на высоту
                return agent.velocity.magnitude / agent.speed;
            }
        }

        protected virtual void Awake() {
            InitComponents();

            hp.onPreDie.AddListener(DisableAgentMotion);
            enemy.onSpawn.AddListener(EnableAgentMotion);
        }

        protected virtual void Start() {
            SetMotionBehavior();
            SetRotationBehavior();
            CreateStackTrace();
        }

        private void Update() {
            //MovementRefresh();
            SetRotation();
        }

        private void CreateStackTrace() {
            Observable.Interval(TimeSpan.FromSeconds(Random.Range(0.4f, 0.6f)), destroyCancellationToken)
                .Where(_ => gameObject.activeInHierarchy)
                .Subscribe(_ => {
                    MovementRefresh();
                });
        }
        
        protected virtual void MovementRefresh() {
            agent.SetDestination(target.position);
        }

        protected virtual void CalculatePath() {
            agent.SetDestination(target.position);
            //motionBehavior.CalculatePath();
        }
        
        protected virtual void SetRotation() {
            rotator.Rotate();
        }

        protected virtual void InitComponents() {
            if (hp.IsUnityNull()) TryGetComponent(out hp);
            if (enemy.IsUnityNull()) TryGetComponent(out enemy);
            if (animator.IsUnityNull()) TryGetComponent(out animator);
            if (agent.IsUnityNull()) GetComponent<Collider>().transform.root.TryGetComponent(out agent);
        }

        public virtual Motion Initialize(float motionSpeed = 1f) {
            this.motionSpeed = motionSpeed;
            
            animator.speed = this.motionSpeed;
            SetAgentSettings();

            return this;
        }

        public void SetTarget(Transform target) => this.target = target;
        

        protected void DisableAgentMotion() {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        protected void EnableAgentMotion() {
            agent.updatePosition = updatePosition;
            agent.updateRotation = updateRotation;
        }

        private void SetAgentSettings() {
            agent.stoppingDistance = stopDistance;
            agent.updatePosition = updatePosition;
            agent.updateRotation = updateRotation;
            
            agent.speed *= motionSpeed;
            agent.angularSpeed *= motionSpeed;
            agent.acceleration *= motionSpeed;
        }

        private void SetMotionBehavior() {
            motionBehavior = new Approach(agent, target);
        }

        private void SetRotationBehavior() {
            rotator = rotate switch {
                RotationBehavior.Default => new DefaultRotator(agent, transform, angularSpeed),
                RotationBehavior.LowTurn => new LowRotator(target, transform, angularSpeed),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public enum MovementBehavior
        {
            None, Approach, KeepDistance
        }

        public enum RotationBehavior
        {
            Default, LowTurn
        }
    }
}