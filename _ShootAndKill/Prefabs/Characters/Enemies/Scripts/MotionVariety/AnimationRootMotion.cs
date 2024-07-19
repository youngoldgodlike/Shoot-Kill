using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemies.MotionVariety
{
    public class AnimationRootMotion : Motion
    {
        [Header("Dependencies")]
        [SerializeField] protected Transform parent;
        [SerializeField] protected bool keepAnimatorStateOnDisable;
        [SerializeField, ReadOnly] protected float baseY;

        [SerializeField, ReadOnly] private float _yAgentDisplacement;
        [SerializeField, ReadOnly] private bool _enableRotation = true;
        
        protected static readonly int Velocity = Animator.StringToHash("Velocity");
        
#pragma warning disable CS0414
        private bool _animatorPass;
#pragma warning restore CS0414

        private void OnValidate() {
            InitComponents();
        }

        protected override void Awake() {
            base.Awake();
            animator.applyRootMotion = true;
            baseY = transform.position.y;
        }

        protected override void Start() {
            base.Start();
            _yAgentDisplacement = agent.nextPosition.y;
            ResetYPosAfterAnimator().Forget();
            animator.keepAnimatorStateOnDisable = keepAnimatorStateOnDisable;
        }

        protected override void MovementRefresh() {
            base.MovementRefresh();
            SetVelocity();
        }

        protected virtual void SetVelocity() {
            //if (velocity == 0) ResetYPos();
            animator.SetFloat(Velocity, velocity);
        }

        protected override void SetRotation() {
            if(_enableRotation) rotator.Rotate();
        }
        

        protected override void InitComponents() {
            base.InitComponents();
            if (parent.IsUnityNull()) parent = GetComponent<Collider>().transform.root;
        }

        [SerializeField] private bool _ignoreYAmplitude = true;
        [SerializeField] private bool _animatorMove = true;
        private void OnAnimatorMove() {
            if(!_animatorMove) return;
            Quaternion rootRotation = animator.rootRotation;
            Vector3 rootPosition = animator.rootPosition; // позиция анимации, которая должна поставится
            var nextAgentY = agent.nextPosition.y - _yAgentDisplacement; // позиция агента, которая должна поставится
            var rootPosY = rootPosition.y - parent.position.y;

            rootPosition.y = nextAgentY;
            parent.position = rootPosition;
            
            var newPos = new Vector3(parent.position.x, 
                rootPosY + nextAgentY,
                parent.position.z);

            transform.position = newPos;
            transform.rotation = rootRotation;

            agent.nextPosition = parent.position;
            
            var agentPos = new Vector3(agent.nextPosition.x, transform.position.y, agent.nextPosition.z); 
            
            // Проверка на совместное передвижение
            var flatVectorMagnitude = new Vector3(Mathf.Abs(agentPos.x - transform.position.x),
                _ignoreYAmplitude ? 0f : agentPos.y - transform.position.y,
                Mathf.Abs(agentPos.z - transform.position.z)).magnitude;
            //if(Mathf.Abs(agentPos.magnitude-transform.position.magnitude) > 1 )
            // if (flatVectorMagnitude > 1f) 
            transform.position = agentPos;
            

            _animatorPass = true;
        }

        public void IgnoreYDiff() => _ignoreYAmplitude = true;
        public void EnableYDiff() => _ignoreYAmplitude = false;
        public void DisableAnimator() => animator.enabled = false;
        public void EnableAnimator() => animator.enabled = true;
        public void DisableAnimatorMove() => _animatorMove = false;
        public void EnableAnimatorMove() => _animatorMove = true;

        [Button("Reset Y Pos")]
        public void ResetYPos() {
            ResetYPosAfterAnimator().Forget();
        }

        [Button("Reset Rotation")]
        public void ResetRotation() {
            ResetRotationAfterAnimator().Forget();
        }

        [Button("Reset Scale")]
        public void ResetScale() {
            ResetScaleAfter().Forget();
        }

        [Button("Rebind animator")]
        public void Rebind() {
            animator.Rebind();
        }

        public void EnableRotation() => _enableRotation = true;
        public void DisableRotation() => _enableRotation = false;
        
        private async UniTaskVoid ResetYPosAfterAnimator() {
            _animatorPass = false;
            await UniTask.WaitUntil(() => _animatorPass = true);
            _animatorPass = false;
            
            var locPos = transform.localPosition;
            transform.localPosition = new Vector3(locPos.x, baseY, locPos.z);
        }

        private async UniTaskVoid ResetRotationAfterAnimator() {
            _animatorPass = false;
            await UniTask.WaitUntil(() => _animatorPass = true);
            _animatorPass = false;

            transform.localRotation = Quaternion.identity;
        }

        private async UniTaskVoid ResetScaleAfter() {
            _animatorPass = false;
            await UniTask.WaitUntil(() => _animatorPass);
            _animatorPass = false;

            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}