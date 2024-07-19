using _Shoot_Kill.Architecture.Scripts.Data;
using Assets.Architecture.Scripts.Utils;
using Assets.UI.Architecture.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Prefabs.Characters.MainHero.Scripts
{
    public class HeroMovement : _Shoot_Kill.Architecture.Scripts.Singleton<HeroMovement>
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private ProgressBarWidget _dashProgressBar;
        [SerializeField] private GameObject _dashParticle;
        [SerializeField] private MatchStatisticController _statisticController;
        [SerializeField] private Renderer _renderer;
        
        [Header("Properties")]
        [SerializeField] private LayerMask _layerRayCast;
        
        private GameSession _gameSession;
        private Rigidbody _rigidBody;
        private Animator _animator;

        private static readonly int IsForward = Animator.StringToHash("isForward");
        private static readonly int IsRight = Animator.StringToHash("isRight");

        private Vector3 direction => _gameSession.matchData.heroData.direction;
        private Cooldown dashDelay => _gameSession.matchData.heroData.dashDelay;
        private FloatDataProperty moveSpeed => _gameSession.matchData.heroData.moveSpeed;
        private float dashForce => _gameSession.matchData.heroData.dashForce;

        [Inject]
        private void Initialize(GameSession gameSession)
        {
            _gameSession = gameSession;
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            CalculateVelocity();
            CalculateRotateDirection();
        }
        
        public void SetDirection(Vector3 direction) =>
            _gameSession.matchData.heroData.SetDirection(direction);
        
        public void Dash()
        {
            if (!dashDelay.isReady) return;
            
            Instantiate(_dashParticle, transform);
            _dashProgressBar.StartCountdown(dashDelay.delay.Value).Forget();
            _rigidBody.AddForce(direction * dashForce * 10f, ForceMode.VelocityChange);
            DisableCollider().Forget();
            dashDelay.Reset();
        }

        private async UniTaskVoid DisableCollider()
        {
            _renderer.material.color = Color.magenta;
            gameObject.layer = LayerMask.NameToLayer("Immunity");
            
            await UniTask.WaitForSeconds(0.5f);
            
            _renderer.material.color = Color.white;
            gameObject.layer = LayerMask.NameToLayer("Hero");
        }

        private void CalculateVelocity()
        {
            var flatVel = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);

            if (!(flatVel.magnitude > moveSpeed.Value))
                _rigidBody.AddForce(direction.normalized * moveSpeed.Value * 10f, ForceMode.Force);
            
            var directionForward = Vector3.Dot(direction, transform.forward);
            var directionRight = Vector3.Dot(direction, transform.right);
            
             _animator.SetFloat(IsForward, directionForward);
             _animator.SetFloat(IsRight, directionRight);
        }

        private void CalculateRotateDirection()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 50, _layerRayCast))
            {
                var lookDirection = hitInfo.point - transform.position;
                lookDirection.y = 0;
               
                var rotation = Quaternion.LookRotation(lookDirection);
                _rigidBody.MoveRotation(rotation);
            }
        }

        public void UseSlowDown(float time) =>
            _gameSession.matchData.heroData.UseSlowdown(time).Forget();
    }
}
