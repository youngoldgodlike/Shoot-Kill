using System;
using UnityEngine;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    [Serializable]
    public struct AbilData 
    {
        [SerializeField] private AbilityTag _tag;
        [SerializeField] private AbilityCell _ability;

        public AbilityCell Ability => _ability;
        public AbilityTag Tag => _tag;
    }

    [Serializable]
    public enum AbilityTag
    {
        SpeedUp,
        DamageUp,
        DashDelay,
        SpeedFireUp,
        Hp,
        ReloadSpeedUp,
        StoreCountUp
    }
}