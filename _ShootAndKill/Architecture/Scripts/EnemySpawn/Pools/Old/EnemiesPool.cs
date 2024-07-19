using System;
using Enemies.EnemiesVariety;

namespace _Shoot_Kill.Architecture.Scripts.EnemySpawn
{
    public class EnemiesPool : PersistencePool<Enemy>
    {
        public EnemiesPool(Func<Enemy> createAction, int capacity) : 
            base(GetAction, ReturnAction, createAction, capacity) { }

        private static void GetAction(Enemy enemy) {
            enemy.onSpawn.Invoke();
            enemy.gameObject.SetActive(true);
        }

        private static void ReturnAction(Enemy enemy) {
            enemy.gameObject.SetActive(false);
        }
        
    }
}