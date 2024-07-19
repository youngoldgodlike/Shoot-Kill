using System;
using UnityEngine;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    [Serializable]
    public class RarityValue<T>
    {
        [field: SerializeField] public AbilityRarity rarity { get; private set; }
        [field: SerializeField] public T value { get; private set; }
    }
}