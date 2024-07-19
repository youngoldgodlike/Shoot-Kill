using R3;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDot : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public readonly ReactiveProperty<bool> isHandled = new(false);

    public void OnPointerDown(PointerEventData eventData) {
        isHandled.Value = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isHandled.Value = false;
    }
}
