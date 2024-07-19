using TMPro;
using UnityEngine;

public class RefreshButton : ActiveButton
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetActiveAndText(bool isActive, string message)
    {
        SetActive(isActive);
        _text.text = message;
    }
}
