using Enemies.MotionVariety;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeBossMovement : AnimationRootMotion
{
    [ShowNativeProperty]
    private float distance => target.IsUnityNull() ? 0f : (target.position - transform.position).magnitude;

    private static readonly int Distance = Animator.StringToHash("Distance");
    
    protected override void SetVelocity() {
        //var velociti = distance > 0.1f ? 1f / 15f * Mathf.Clamp(distance, 1f, 15f) : 0f;
        var velociti = Mathf.Clamp(distance, 0, 14);

        animator.SetFloat(Distance, velociti);
    }
}
