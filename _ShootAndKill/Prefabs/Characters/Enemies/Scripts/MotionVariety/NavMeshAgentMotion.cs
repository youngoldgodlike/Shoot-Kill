using UnityEngine;

namespace Enemies.MotionVariety
{
    public class NavMeshAgentMotion : Motion
    {
        private float _speed;
        protected override void Awake() {
            base.Awake();
            animator.applyRootMotion = false;
            _speed = agent.speed;
        }

        protected void Update() {
            if (agent.remainingDistance < agent.stoppingDistance)
                agent.velocity = Vector3.zero;
        }
    }
}