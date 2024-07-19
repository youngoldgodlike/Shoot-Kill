using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.Attachments
{
    public class SkinMeshChanger : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _hitMat;

        public void SetHitMat(float duration) {
            _meshRenderer.material = _hitMat;
            Invoke(nameof(SetDefaultMat), duration);
        }

        [Button("Hit Material")]
        public void SetDeathMat() => _meshRenderer.material = _hitMat;

        [Button("Default Material")]
        public void SetDefaultMat() {
            _meshRenderer.material = _defaultMaterial;
        }
    }
}
