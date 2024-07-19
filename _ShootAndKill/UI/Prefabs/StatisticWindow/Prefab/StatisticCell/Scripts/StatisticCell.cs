using Assets.UI.Prefabs.Abilities.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _outlineImage;
    
    [field: SerializeField] public AbilityTag id { get; private set; }
    [field: SerializeField] public AbilityRarity rarity { get; private set; }

    private int _currentCount = 0;

    public void Init(AbilityCell abilityCell)
    {
        _image.sprite = abilityCell.currentDefaultIcon.sprite;
        rarity = abilityCell.rarity;
        _outlineImage.sprite = abilityCell.outliningImage.sprite;
        id = abilityCell.id;
    }

    public void SetSiblingIndex(int index)
    {
        transform.SetSiblingIndex(index);
    }
    
    public void UpdateInfo(int count = 1)
    {
        _currentCount += count;
        _text.text = $"X{_currentCount}";
    }
    
}