using UnityEngine;

namespace Assets.Architecture.Scripts.Components
{
    public class RotateObjectAroundItself : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;
        private void Update()
        {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }
    }
}