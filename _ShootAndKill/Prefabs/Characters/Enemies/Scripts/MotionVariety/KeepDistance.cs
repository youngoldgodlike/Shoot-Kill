using UnityEngine;
using UnityEngine.AI;

namespace Enemies.MotionVariety
{
    public class KeepDistance : MotionBehavior
    {
        public KeepDistance(NavMeshAgent agent, Transform target) : base(agent, target) {
        }

        public override void CalculatePath() {
        }
    }
}