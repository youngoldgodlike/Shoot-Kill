using System;
using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts;

namespace _Shoot_Kill.Architecture.Scripts.EnemySpawn
{
    public class EnemyProjectilesPool : ExpandablePool<EnemyProjectile>
    {
        public EnemyProjectilesPool(Func<EnemyProjectile> createAction, int capacity) :
            base(GetAction, ReturnAction, () => createAction(), capacity) {
        }
        
        private static void GetAction(EnemyProjectile obj) {
            obj.gameObject.SetActive(true);
        }

        private static void ReturnAction(EnemyProjectile obj) {
            obj.gameObject.SetActive(false);
        }
    }
}