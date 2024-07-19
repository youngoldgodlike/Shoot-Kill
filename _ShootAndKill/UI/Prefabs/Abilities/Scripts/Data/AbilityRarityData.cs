using System;
using UnityEngine;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    [Serializable]
    public class AbilityRarityData 
    {
        [field: SerializeField] public AbilityRarity rarity;
        [field: SerializeField] public Sprite outliningImage;
    }
}