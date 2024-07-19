using UnityEngine;

namespace _Shoot_Kill.UI.Prefabs.LoseMenu.Scripts
{
    public class LoseMenuLink : MonoBehaviour
    {
        public void OnClose() => LoseMenuController.Instance.onClose?.Invoke();
    }
}