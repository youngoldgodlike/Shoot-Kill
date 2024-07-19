using TMPro;
using UnityEngine;

namespace Assets.UI.Architecture.Scripts
{
    public class TimeText : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI text;
        
        public void SetTimeText(float count)
        {
            var min = (int)count / 60;
            var sec = (int)count % 60;

            if (sec < 10)
                text.text = $"{min} : 0{Mathf.Round(sec)}";
            else 
                text.text = $"{min} : {Mathf.Round(sec)}";
        }

    }
}