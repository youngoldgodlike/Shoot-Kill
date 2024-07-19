using System.Collections.Generic;
using Assets.UI.Prefabs.Abilities.Scripts;
using UnityEngine;

public class RangeStatisticAbilities : MonoBehaviour
{
    [field: SerializeField] public AbilityTag id { get; private set; }
    [SerializeField] private StatisticCell _cellPrefab;

    private Dictionary<AbilityRarity, StatisticCell> _statisticCells;

    public void Init( AbilityCell abilityCell)
    {
        _statisticCells = new();
        
        id = abilityCell.id;
        AddCell(abilityCell);
    }

    public void UpdateStatistic(AbilityCell abilityCell)
    {

        foreach (var statisticCell in _statisticCells)
        {
            if (statisticCell.Key != abilityCell.rarity) continue;
            
            statisticCell.Value.UpdateInfo();
            return;
        }

        AddCell(abilityCell);
        Streamline();
    }

    private StatisticCell AddCell(AbilityCell abilityCell)
    {
        var cell = Instantiate(_cellPrefab, transform);
        cell.Init(abilityCell);
        cell.UpdateInfo();
       
        _statisticCells.Add(cell.rarity, cell);

        return cell;
    }

    private void Streamline()
    {
        foreach (var cell in _statisticCells)
        {
            switch (cell.Key)
            {
                case AbilityRarity.Common:
                    cell.Value.SetSiblingIndex(0);
                    break;
                case AbilityRarity.Rare:
                    cell.Value.SetSiblingIndex(1);
                    break;
                case AbilityRarity.Mythical:
                    cell.Value.SetSiblingIndex(2);
                    break;
            }
        }
    }
}
