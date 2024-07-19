using NaughtyAttributes;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _triggerColor;

    private void Awake()
    {
         _material.EnableKeyword("_EMISSION");
    }
    
    [Button("SetTriggerColor")]
    public void SetTriggerColor()
    {
        _material.SetColor("_EmissionColor", _triggerColor);
    }
    [Button("SetDefaultColor")]
    public void SetDefaultColor()
    {
        _material.SetColor("_EmissionColor", _defaultColor);
    }
    public void SetColor(Color color) => _material.SetColor("_EmissionColor", color);
    
}
