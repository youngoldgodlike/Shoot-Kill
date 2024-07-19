using TMPro;
using UnityEngine;

public class StatisticText : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;

    public void UpgradeInfo(string message)
    {
        text.text = message;
    }

}
