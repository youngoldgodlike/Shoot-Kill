using TMPro;
using UnityEngine;

public class AdText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    
    private AdMenu _adMenu;

    private void Awake() =>
        _adMenu = GetComponent<AdMenu>();
    
    private void OnEnable() =>
        _adMenu.onTickShow += SetText;
    
    private void OnDisable() =>
        _adMenu.onTickShow -= SetText;
    
    private void SetText(float value) =>
        _text.text = $"{Mathf.Round(value)}";
    
}
