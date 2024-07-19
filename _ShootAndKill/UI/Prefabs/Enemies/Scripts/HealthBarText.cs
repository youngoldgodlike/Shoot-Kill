using TMPro;
using UnityEngine;

namespace _Shoot_Kill.UI.Prefabs.Enemies.Scripts
{
    public class HealthBarText : MonoBehaviour
    {
        [SerializeField] private GameObject _header;
        [SerializeField] private TextMeshProUGUI _text;

        private void Start() {
            _text.text = _header.name;
        }
        
    }
}
