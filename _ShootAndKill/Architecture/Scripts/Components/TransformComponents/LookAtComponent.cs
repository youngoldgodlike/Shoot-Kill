using UnityEngine;

namespace Assets.Architecture.Scripts.Components
{
    public class LookAtComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        private void Update()
        {
            transform.LookAt(_target);
        }
    }
}
