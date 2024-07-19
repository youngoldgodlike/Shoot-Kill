using UnityEngine;

namespace _Shoot_Kill.UI.Prefabs.Menu.PauseMenu.Scripts
{
    public class PauseLink : MonoBehaviour
    {
        public void OnClose() => PauseMenu.Instance.onClose?.Invoke();
    }
}