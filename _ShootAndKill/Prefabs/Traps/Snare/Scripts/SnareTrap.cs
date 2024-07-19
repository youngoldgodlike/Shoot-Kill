using _Shoot_Kill.Prefabs.Traps.Scripts;
using Assets.Prefabs.Characters.MainHero.Scripts;
using UnityEngine;

public class SnareTrap : Trap
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HeroMovement movement))
        {
            movement.UseSlowDown(3f);
            animator.SetTrigger(isTouch);
        }
    }
}
