using _Shoot_Kill.Architecture.Scripts.Data;
using Assets.Prefabs.Characters.MainHero.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroConfig", menuName = "Configs/HeroConfig")]

public class HeroConfig : ScriptableObject
{
    [field: SerializeField] public ConfigData<float> moveSpeed { get; private set; }
    [field: SerializeField] public ConfigData<float> dashDelay { get; private set; }
    [field: SerializeField] public ConfigData<float> health { get; private set; }
    [field: SerializeField] public float dashForce { get; private set; }
}
