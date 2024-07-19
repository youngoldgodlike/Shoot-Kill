using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class AbilityFloatCell : AbilityCell
    {
        [SerializeField] private List<RarityValue<float>> _rarityValues;
        
        protected float value;

        public override void SetRarity(AbilityRarity rarity,  AbilityRarityDataScriptableObject rarityData )
        {
            base.SetRarity(rarity, rarityData);

            foreach (var rar in _rarityValues)
            {
                if (rar.rarity != rarity) continue;

                value = rar.value;
            }
            
            SetText();
        }
    }
}