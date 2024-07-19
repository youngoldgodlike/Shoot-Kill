using Characters;
using Enemies.DmgInfliction;
using UnityEngine;

namespace Enemies.DmgInflictionVariety
{
    public class BaseAttack : DamageInfliction
    {
        [SerializeField] private Transform _middlePoint;

        private Ray forward => new(_middlePoint.position, target.position - _middlePoint.position);
        
        public override void Assault() {
            if (!Physics.Raycast(forward, out var info, attackDistance + 0.5f, targetMask)) return;
            
            info.transform.GetComponent<IDamageable>().DealDamage(damageDeal);
            Debug.DrawRay(_middlePoint.position, _middlePoint.forward * attackDistance, Color.green, 1f);
        }
    }
}