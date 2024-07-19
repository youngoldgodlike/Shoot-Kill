using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using R3;
using Unity.VisualScripting;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class SpawnPosition : MonoBehaviour
    {
        [SerializeField,ReadOnly] private Transform _player;
        [SerializeField,ReadOnly] private bool _active = true;
        [SerializeField, ReadOnly] private string _whoCollided;
        [ShowNativeProperty] public float magnitude => _player.IsUnityNull() ? 0f : (_player.position - position).magnitude;
        public bool active => _active;
        public Vector3 position => transform.position;

        private void Awake() => Destroy(GetComponent<MeshRenderer>());
        
        public void SetPlayer(Transform player) => _player = player;
        
        private void OnTriggerEnter(Collider other) {
            _whoCollided = $"ENTER: {other.name}";
            _active = false;
        }
        
        private void OnTriggerExit(Collider other) {
            _whoCollided = $"EXIT: {other.name}";
            _active = true;
        }
    }
}