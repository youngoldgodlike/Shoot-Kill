using UnityEngine;

namespace Assets.Prefabs.Characters.MainHero.Scripts.XPSystem
{
    [CreateAssetMenu(menuName = "Configs/ExpSystemConfig", fileName = "ExpSystemConfig")]
    public class ExpSystemConfig : ScriptableObject
    {
        [field: SerializeField] public float maxExp { get; private set; }
        [field: SerializeField] public int growthFactor { get; private set; }
    }
}