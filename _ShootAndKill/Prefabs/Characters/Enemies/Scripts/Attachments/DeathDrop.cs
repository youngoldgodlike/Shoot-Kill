using System;
using System.Collections.Generic;
using System.Linq;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;
using Assets.Prefabs.ExpStar.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.Attachments
{
    public class DeathDrop : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Transform _dropPlace;
        [SerializeField] private List<DropList> _dropItems;
        
        private ExpStarPool _pool;

        private void OnValidate() {
            if (_health.IsUnityNull()) TryGetComponent(out _health);
        }

        private void Awake() {
            _health.onDieProcessEnd.AddListener(() => {
                foreach (var item in from item in _dropItems let toDrop = Random.Range(0, 100) <= item.dropChance where toDrop select item) {
                    Drop(item.item);
                }
            });
        }

        [Inject]
        private void Initialize(ExpStarPool pool) {
            try {
                _pool = pool;
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private void Drop(GameObject obj) => Instantiate(obj, _dropPlace.position, Quaternion.identity);
        
        public void DropExp(float quantity) {
            var exp = _pool.Get();
            exp.transform.position = _dropPlace.position;
            exp.Initialize(quantity);
        }
    }

    [Serializable]
    public struct DropList
    {
        [SerializeField] private GameObject _item;
        [SerializeField, Range(0, 100)] private int _dropChance;

        public GameObject item => _item;
        public int dropChance => _dropChance;
    }
}
