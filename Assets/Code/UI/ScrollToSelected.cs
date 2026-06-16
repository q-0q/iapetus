using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI; 

[RequireComponent(typeof(ScrollRect))]
public class ScrollToSelected : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 70f; 
    [SerializeField] private float padding = 10f; // Extra padding to keep elements away from the absolute edge

    private ScrollRect scrollRect;
    private RectTransform viewportRect;
    private RectTransform contentRect;
    private GameObject lastSelected;
    private float targetY;
    private InputSystemUIInputModule uiInputModule;

    private bool active = false;
    private PlayerInput _playerInput;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        _playerInput = GetComponent<PlayerInput>();
        viewportRect = scrollRect.viewport;
        contentRect = scrollRect.content;
        targetY = contentRect.anchoredPosition.y;

        if (EventSystem.current != null)
        {
            uiInputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
        }
    }

    void Update()
    {
        if (_playerInput.actions["Move"].ReadValue<Vector2>().magnitude > 0.01) active = true;
        if (_playerInput.actions["Look"].ReadValue<Vector2>().magnitude > 0.01) active = false;
        if (!active) return;
        
        if (Mathf.Abs(scrollRect.velocity.y) > 0.1f) return;
        
        if (EventSystem.current == null) return;
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected == null) return;
        if (!currentSelected.transform.IsChildOf(contentRect)) return;
        RectTransform selectedRect = currentSelected.GetComponent<RectTransform>();
        if (selectedRect == null) return;
        DetermineTargetScrollPosition(selectedRect);
        
        float currentY = Mathf.Lerp(contentRect.anchoredPosition.y, targetY, Time.deltaTime * scrollSpeed);
        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, currentY);
    }

    private bool IsNavigatingViaKeyboardOrController()
    {
        if (uiInputModule == null)
        {
            uiInputModule = EventSystem.current.currentInputModule as InputSystemUIInputModule;
            if (uiInputModule == null) return false;
        }

        var moveAction = uiInputModule.move.action;
        if (moveAction != null && moveAction.triggered)
        {
            return true;
        }

        return false;
    }

void DetermineTargetScrollPosition(RectTransform target)
{

    if (GetComponentsInChildren<Selectable>()[0].gameObject == EventSystem.current.currentSelectedGameObject)
    {
        targetY = 0;
        return;
    }
    
    
    // 1. Get the local position of the selected element's pivot inside the Content panel
    Vector3 targetLocalPos = contentRect.InverseTransformPoint(target.position);

    // 2. Calculate the exact center of the element in content local space, completely bypassing pivot variance
    float elementCenterY = targetLocalPos.y + (target.rect.height * (target.pivot.y - 0.5f));

    // 3. Define the true top and bottom edges of the element relative to its center
    float halfHeight = target.rect.height * 0.5f;
    float targetTop = elementCenterY + halfHeight;
    float targetBottom = elementCenterY - halfHeight;

    // 4. Define the current viewable boundaries of the viewport relative to the content
    float currentTopBound = -contentRect.anchoredPosition.y;
    float currentBottomBound = -contentRect.anchoredPosition.y - viewportRect.rect.height;

    // 5. Structural limits of your Vertical Layout Group content
    float contentTopLimit = 0f;
    float contentBottomLimit = -contentRect.rect.height;

    // Calculate exactly how much extra room exists structurally in the entire list
    float spaceAvailableAbove = Mathf.Abs(targetTop - contentTopLimit);
    float spaceAvailableBelow = Mathf.Abs(targetBottom - contentBottomLimit);

    // 6. Evaluate movement with correctly mirrored arithmetic signs
    if (targetTop > (currentTopBound - padding)) // User is navigating UP toward the top edge
    {
        float appliedTopPadding = (spaceAvailableAbove >= padding) ? padding : spaceAvailableAbove;
        
        // Subtracting pushes targetY down, revealing content above
        targetY = -targetTop - appliedTopPadding;
    }
    else if (targetBottom < (currentBottomBound + padding)) // User is navigating DOWN toward the bottom edge
    {
        float appliedBottomPadding = (spaceAvailableBelow >= padding) ? padding : spaceAvailableBelow;
        
        // FIXED: Adding pushes targetY up, pulling up the content below to tease the next element
        targetY = -targetBottom - viewportRect.rect.height + appliedBottomPadding;
    }
    else
    {
        // The element, along with its padding requirements, is safely inside the viewport window.
        targetY = contentRect.anchoredPosition.y;
        return;
    }

    // 7. Hard structural clamp so Unity's ScrollRect doesn't violently snap or bounce at the extreme limits
    float minScrollY = 0f;
    float maxScrollY = Mathf.Max(0f, contentRect.rect.height - viewportRect.rect.height);
    targetY = Mathf.Clamp(targetY, minScrollY, maxScrollY);

    if (scrollSpeed <= 0)
    {
        contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, targetY);
    }
}
}