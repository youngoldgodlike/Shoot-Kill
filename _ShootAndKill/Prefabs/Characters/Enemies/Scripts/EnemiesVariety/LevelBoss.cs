using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.HealthVariety;
using NaughtyAttributes;
using UnityEngine;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.EnemiesVariety
{
    // УДОЛИТЬ
    [RequireComponent(typeof(Health))]
    public class LevelBoss : global::Enemies.EnemiesVariety.Enemy
    {
        private void Awake() {
            //_hp.onDieProcessEnd.AddListener(() => { Debug.Log("GAME WIN"); });
        }
    }
}