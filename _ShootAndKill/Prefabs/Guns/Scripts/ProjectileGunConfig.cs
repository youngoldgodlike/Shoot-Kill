using Assets.Prefabs.Characters.MainHero.Scripts;
using UnityEngine;

namespace Assets.Prefabs.Guns.Scripts
{
    [CreateAssetMenu(fileName = "ProjectileGunConfig", menuName = "Configs/ProjectileGunConfig")]
    
    public class ProjectileGunConfig : ScriptableObject
    {
        [field: SerializeField] public ConfigData<int> storeCount { get; private set; }
        [field: SerializeField] public ConfigData<float> damage { get; private set; }
        [field: SerializeField] public ConfigData<float> reloadSpeed { get; private set; }
        [field: SerializeField] public ConfigData<float> rate { get; private set; }
        [field: SerializeField] public float abilityTime { get; private set; }
    }
}
