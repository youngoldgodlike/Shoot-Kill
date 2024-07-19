using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts
{
    public class BaseHealth : Health
    {
        protected override void OnZeroHealth() {
            onPreDie.Invoke();
            _animator.SetBool(Death, true);
        }
    }
}