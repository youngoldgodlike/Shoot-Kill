using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.Attachments
{
    public class MeshChanger : MonoBehaviour
    {
        [SerializeField] private bool _severalRenderers;
        [SerializeField, HideIf(nameof(_severalRenderers))] 
        private MeshRenderer _meshRenderer;
        [SerializeField, ShowIf(nameof(_severalRenderers))] 
        private List<MeshRenderer> _meshRenderers;

        [SerializeField] private bool _severalMaterials;
        [SerializeField, HideIf(nameof(_severalMaterials))]
        private Material _defaultMaterial;
        [SerializeField, ShowIf(nameof(_severalMaterials))]
        private List<Material> _defaultMaterials;
        
        [SerializeField] private Material _hitMat;

        [SerializeField] private bool _interruption = true;
        private bool _isChanged;

        // private void OnValidate() {
        //     if (_severalMaterials) {
        //         _defaultMaterials = new List<Material>(_meshRenderers.Count);
        //     }
        // } // dont working idk why

        public void SetHitMat(float duration) {
            if (!AllowToChange()) return;
            
            SetHitMat();

            _isChanged = true;
            Invoke(nameof(SetDefaultMat), duration);
        }

        private bool AllowToChange() {
            if (_interruption) return true;
            return !_isChanged;
        }
        
        [Button("Hit Material")]
        private void SetHitMat() {
            if (_severalRenderers) ChangeRenderersMaterial(_hitMat);
            else _meshRenderer.material = _hitMat;
        }

        private void ChangeRenderersMaterial(Material material) {
            if (!_severalMaterials) {
                foreach (var render in _meshRenderers) {
                    render.material = material;
                }
            }
            else {
                for (int i = 0; i < _meshRenderers.Count; i++) {
                    _meshRenderers[i].material = _defaultMaterials[i];
                }
            }
        }

        [Button("Default Material")]
        public void SetDefaultMat() {
            if (_severalRenderers) ChangeRenderersMaterial(_defaultMaterial);
            _meshRenderer.material = _defaultMaterial;
        }
    }
}