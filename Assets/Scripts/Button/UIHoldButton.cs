using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// Attach to a UI Image/Button. Fires events on press and release so it supports
// "hold to move" properly — regular Unity Button.OnClick only fires on release.
[RequireComponent(typeof(RectTransform))]
public class UIHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onPress;
    public UnityEvent onRelease;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPress?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onRelease?.Invoke();
    }

    // If the button is disabled/hidden while held, make sure we fire the release
    void OnDisable()
    {
        onRelease?.Invoke();
    }
}
