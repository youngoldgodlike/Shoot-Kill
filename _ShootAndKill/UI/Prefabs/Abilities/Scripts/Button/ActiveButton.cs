using UnityEngine;

public class ActiveButton : MonoBehaviour
{
    [SerializeField] private GameObject _button;
    [SerializeField] private GameObject _image;

    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            _button.SetActive(true);
            _image.SetActive(false);
        }
        else
        {
            _button.SetActive(false);
            _image.SetActive(true);
        }
    }
}
