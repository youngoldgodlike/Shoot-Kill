using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.EnemiesVariety;
using Enemies.EnemiesVariety;
using UnityEngine;

namespace _Shoot_Kill.UI.Prefabs.Enemies.Scripts
{
    public class HealthBarBoss : HealthBar
    {
        [SerializeField] private Enemy _enemy;

        private void Start() {
            _background.SetActive(true);
            _enemy.health.onPreDie.AddListener(() => _background.SetActive(false));
            //_enemy.onSpawn.AddListener(()=>_background.SetActive(true));
        }
    }
}
