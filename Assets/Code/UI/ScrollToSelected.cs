using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class ScrollToSelected : MonoBehaviour
{
    private float scrollSpeed = 70f; 

    private ScrollRect scrollRect;
    private RectTransform viewportRect;
    private RectTransform contentRect;
    private GameObject lastSelected;
    private float targetY;
    private bool active = false;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        viewportRect = scrollRect.viewport;
        contentRect = scrollRect.content;
        targetY = contentRect.anchoredPosition.y;
    }

    void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected == null) return;

        // 1. Check if the newly selected object is a child of our scroll content
        if (currentSelected != lastSelected && currentSelected.transform.IsChildOf(contentRect))
        {
            RectTransform selectedRect = currentSelected.GetComponent<RectTransform>();
            if (selectedRect != null)
            {
                active = true;
                DetermineTargetScrollPosition(selectedRect);
            }
            lastSelected = currentSelected;
        }

        // 2. Prevent fighting: If the user manually scrolls with a mouse wheel, yield to it
        if (Mathf.Abs(scrollRect.velocity.y) > 0.1f)
        {
            active = false;
            targetY = contentRect.anchoredPosition.y;
        }
        // 3. Otherwise, smoothly interpolate to the target position
        else if (scrollSpeed > 0 && Mathf.Abs(contentRect.anchoredPosition.y - targetY) > 0.01f && active)
        {
            // var offset = 
            float currentY = Mathf.Lerp(contentRect.anchoredPosition.y, targetY, Time.deltaTime * scrollSpeed);
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, currentY);
        }
    }

    void DetermineTargetScrollPosition(RectTransform target)
    {
        // Convert the target button's position into the Viewport's local space coordinates
        Vector3 targetLocalPos = viewportRect.InverseTransformPoint(target.position);

        // Calculate where the top and bottom bounds of the button are
        float targetTop = targetLocalPos.y + (target.rect.height * (1f - target.pivot.y));
        float targetBottom = targetLocalPos.y - (target.rect.height * target.pivot.y);

        // Get viewport boundaries
        float viewportTop = viewportRect.rect.yMax;
        float viewportBottom = viewportRect.rect.yMin;

        // Check if the item is hiding past the top boundary
        if (targetTop > viewportTop)
        {
            float overflow = targetTop - viewportTop;
            targetY = contentRect.anchoredPosition.y - overflow;
        }
        // Check if the item is hiding past the bottom boundary
        else if (targetBottom < viewportBottom)
        {
            float overflow = viewportBottom - targetBottom;
            targetY = contentRect.anchoredPosition.y + overflow;
        }

        // If scroll speed is set to 0 or less, snap instantly instead of animating
        if (scrollSpeed <= 0)
        {
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, targetY);
        }
    }
}