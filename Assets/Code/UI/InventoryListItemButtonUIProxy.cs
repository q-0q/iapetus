using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryListItemButtonUIProxy : MonoBehaviour, 
    IPointerClickHandler, 
    IPointerEnterHandler, 
    ISelectHandler, 
    IDeselectHandler, 
    ISubmitHandler
{

    public event Action OnMakeSelected;
    public event Action OnMakeDeselected;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnMakeSelected?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMakeSelected?.Invoke();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnMakeSelected?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnMakeDeselected?.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnMakeSelected?.Invoke();
    }
}