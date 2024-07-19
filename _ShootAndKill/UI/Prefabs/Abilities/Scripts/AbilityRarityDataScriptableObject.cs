using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    [CreateAssetMenu(fileName = "AbilityRarityDataScriptableObject", menuName = "Storages/AbilityRarityDataScriptableObject")]
    public class AbilityRarityDataScriptableObject : ScriptableObject
    {
        [field: SerializeField] public List<AbilityRarityData> abilityRarityDatas;
    }
}