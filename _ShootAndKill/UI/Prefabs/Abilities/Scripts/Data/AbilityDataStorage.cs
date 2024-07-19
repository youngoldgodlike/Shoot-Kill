using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.Prefabs.Abilities.Scripts
{
    
    [CreateAssetMenu(fileName = "AbilityDataStorage", menuName = "Storages/AbilityDataStorage")]
    public class AbilityDataStorage : ScriptableObject
    {
        [field: SerializeField] public List<AbilData> abilitiesData { get; private set; }
    }
}