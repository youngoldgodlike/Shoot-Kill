using Characters;
using NaughtyAttributes;
using UnityEngine;

namespace Enemies.DmgInfliction
{
    public class CollisionDamage : DamageInfliction
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private float _repulsionPower = 5f;

        public override void Assault() {
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != targetMask.ToLayer()) {
//                Debug.Log($"COLLISED WITH NOT PLAYER. {other.gameObject.layer} != {targetMask.ToLayer()}");
                return;
            }
            
//            Debug.Log($"{name} Collided With {other.name}");

            var targetTransform = other.transform;
            var targetPos = targetTransform.position;
            var transform1 = transform;
            var myPos = transform1.position;

            var dir = targetPos - myPos;
            var forward = transform1.forward;
            var right = transform1.right;
            
            var dotRight = Vector3.Dot(dir, right);
            var dotLeft = Vector3.Dot(forward, -right);
            var repulsionDir = dotRight > dotLeft ? right : -right;
            Debug.DrawLine(myPos, repulsionDir * 5f, Color.blue, 5f);

            repulsionDir = Vector3.ProjectOnPlane(repulsionDir, Vector3.up);
            repulsionDir *= 2;
            repulsionDir += transform.up;
            Debug.DrawRay(myPos, repulsionDir, Color.green, 3f);
            
            other.attachedRigidbody.AddForce(repulsionDir * _repulsionPower, ForceMode.Impulse);

            if (other.TryGetComponent(out IDamageable dmg)) dmg.DealDamage(damageDeal);
            else Debug.LogError($"WATAFAK!? WHERE COMPONENT IN {other.gameObject.name}??", this);
        }
    }
}