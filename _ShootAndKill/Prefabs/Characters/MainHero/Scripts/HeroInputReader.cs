using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Prefabs.Characters.MainHero.Scripts
{
    public class HeroInputReader : MonoBehaviour
    {
        [SerializeField] private HeroController _controller;
        [SerializeField] private HeroMovement _movement;
        
        private GameSession _gameSession;

        [Inject]
        private void Initialize(GameSession gameSession)
        {
            _gameSession = gameSession;
        }
        
        public void OnMovement(InputAction.CallbackContext context)
        {
            if (_gameSession.UIIsActive.Value) return;

            var direction = context.ReadValue<Vector2>().normalized;
            _movement.SetDirection(new Vector3(direction.x, 0f, direction.y));
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (_gameSession.UIIsActive.Value) return;
            
            if (context.performed)
                _movement.Dash();
        }

        public void OnUseGunAbility(InputAction.CallbackContext context)
        {
            if (_gameSession.UIIsActive.Value) return;
            
            if (context.performed)
                _controller.currentGun.UseAbility().Forget();
        }

        private void Update()
        {
            if (_gameSession.UIIsActive.Value) return;
           
            if (Input.GetMouseButton(0))
                _controller.currentGun.PerformAttack();

            if (Input.GetKeyDown(KeyCode.R))
                _controller.currentGun.Reload().Forget();
        }
    }
}
