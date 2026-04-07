using UnityEngine;
using UnityEngine.EventSystems;

public class MapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public RectTransform marker;

    public Vector2 lastClickPosition;
    public bool hasClicked = false;
    public bool canClick = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick) return;
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        lastClickPosition = localPoint;
        hasClicked = true;

        marker.anchoredPosition = localPoint;
        marker.gameObject.SetActive(true);
    }

    public void ResetClick()
    {
        hasClicked = false;
        marker.gameObject.SetActive(false);
    }
}