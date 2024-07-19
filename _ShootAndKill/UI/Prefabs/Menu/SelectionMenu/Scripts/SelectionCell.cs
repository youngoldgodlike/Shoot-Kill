using Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _Shoot_Kill.UI.Prefabs.Menu.SelectionMenu
{
    public class SelectionCell : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        public WeaponData data { get; private set ; }
        public GameObject model { get; private set; }
        public bool isLock { get; private set; }

        public void Init(WeaponData weaponData, Transform parent)
        {
            data = weaponData;
            image.sprite = weaponData.sprite; 
            isLock = weaponData.isLock;

            if (!isLock)
            {
                GetComponent<RectTransform>().SetWidth(200);
                GetComponent<RectTransform>().SetHeight(200);
            }
            
            model = Instantiate(weaponData.modelForSelectionMenu, parent);
            model.transform.localPosition = Vector3.zero;
        }
    }
}