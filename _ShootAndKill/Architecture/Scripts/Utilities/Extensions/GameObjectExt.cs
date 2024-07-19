using UnityEngine;

namespace _Shoot_Kill.Architecture.Scripts.Utilities.Extensions
{
    public static class GameObjectExt 
    {
        public static bool TryGetComponentInParent<T>(this GameObject go, out T component) {
            if (go.transform.parent.TryGetComponent(out component)) {
                return true;
            }

            component = default(T);
            return false;
        }
    }
}
