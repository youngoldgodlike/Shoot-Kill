using UnityEngine;

namespace Assets.UI.Prefabs.MainMenu.Scripts
{
    public class OptionalMenuController : Menu<OptionalMenuController>
    {
        [SerializeField] private GameObject _menuWindow;
        [SerializeField] private GameObject _settingsWindow;

        public void ExitGame()
        {
            onClose = () =>  Application.Quit();
            animator.SetTrigger(IsClose);
        } 

        public void OpenSettings()
        {
            _menuWindow.SetActive(false);
            _settingsWindow.SetActive(true);
        }
        
    }
}
