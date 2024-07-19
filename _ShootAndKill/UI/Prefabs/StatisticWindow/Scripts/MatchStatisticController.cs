using System.Collections.Generic;
using Assets.UI.Prefabs.Abilities.Scripts;
using TMPro;
using UnityEngine;
using Zenject;

public class MatchStatisticController : MonoBehaviour
{
    [SerializeField] private RangeStatisticAbilities _rangeStatisticPrefab;
    
    [SerializeField] private StatisticText _textPrefab;
    
    [SerializeField] private Transform _scrollRangesContainer;
    [SerializeField] private Transform _scrollTextContainer;

    private readonly List<RangeStatisticAbilities> _statisticRanges = new();
    private List<TextMeshProUGUI> _statisticTexts;

    private GameSession _gameSession;

    [Inject]
    private void Initialize(GameSession gameSession)
    {
        _gameSession = gameSession;
    }
    
    private void Start()
    {
        _gameSession.matchData.SetStatisticTexts(this);
    }

    public void UpdateStatistic(AbilityCell abilityCell)
    {
        foreach (var range in _statisticRanges)
        {
            if (range.id != abilityCell.id) continue;
            
            range.UpdateStatistic(abilityCell);
            return;
        }

        CreateStatisticRange(abilityCell);
    }

    private RangeStatisticAbilities CreateStatisticRange(AbilityCell cell)
    {
        var range = Instantiate(_rangeStatisticPrefab, _scrollRangesContainer);
        range.Init(cell);
        
        _statisticRanges.Add(range);
        
        return range;
    }

    public StatisticText SpawnText()
    {
        var text = Instantiate(_textPrefab, _scrollTextContainer);
        return text;
    }

    
}
