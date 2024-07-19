using UnityEngine;

namespace Assets.UI.Prefabs.Settings.Scripts
{
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private GameObject _menuWindow;
        [SerializeField] private GameObject _settingsWindow;

        public void BackMenu()
        {
            _settingsWindow.SetActive(false);
            _menuWindow.SetActive(true);
        }
    }
}
