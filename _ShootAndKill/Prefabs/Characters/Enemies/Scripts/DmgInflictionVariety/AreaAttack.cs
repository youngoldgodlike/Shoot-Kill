using System;
using System.Linq;
using Characters;
using Enemies.DmgInfliction;
using NaughtyAttributes;
using UnityEngine;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.DmgInflictionVariety
{
    public class AreaAttack : DamageInfliction
    {
        [Header("AreaAttack Params")] 
        [SerializeField] private ParticleSystem _attackParticle;
        [SerializeField] private float _damageRadius;
        [SerializeField] private Transform _vfxPoint;
        [SerializeField] private bool _changeVFXRadius;
        [SerializeField] private bool _usePhysicsWhenBoom;
        [SerializeField, ShowIf(nameof(_changeVFXRadius))] private float _vfxRadius;
        [SerializeField] private float _repulsiveTorque;
        
        public float radius => _damageRadius;

        public override void Assault() {
            var targets = Physics.OverlapSphere(position, _damageRadius, targetMask);
            
            #region VFX

            var vfx = Instantiate(_attackParticle, _vfxPoint.position, Quaternion.identity);
            var emission = vfx.emission;
            var shape = vfx.shape;
            var main = vfx.main;

            if (_changeVFXRadius) {
                shape.radius = _vfxRadius;
                main.startSize = _vfxRadius;
            }

            emission.enabled = true;
            Destroy(vfx.gameObject, 3f);

            #endregion

            if(targets.Length == 0) return;

            targets.ToList().ForEach(col => {
                try {
                    if(_usePhysicsWhenBoom)
                        col.attachedRigidbody.AddExplosionForce(_repulsiveTorque, _vfxPoint.position, _damageRadius, 1f,
                            ForceMode.Impulse);
                    if(col.TryGetComponent<IDamageable>(out var dmg)) dmg.DealDamage(damageDeal);
                }
                catch (Exception e) {
                    Debug.Log(e);
                }
            });
        }

        private void CreateDebugObject() {
            var obj = Instantiate(new GameObject("AreaAttackDebug"), _vfxPoint.position, Quaternion.identity);
            var col = obj.AddComponent<SphereCollider>();
            col.radius = _damageRadius;
            col.isTrigger = false;
            Destroy(obj, 5f);
        }
    }
}
