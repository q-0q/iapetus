using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListItem : MonoBehaviour
{

    public class InventoryListItemData
    {
        public string id;
        public string displayName;
        public string description;
        public string subText;
    }
    
    private CanvasGroup _canvasGroup;
    public InventoryListItemData Data;

    public static event Action<InventoryListItemData> OnInventorySlotClicked;
    public static event Action<InventoryListItemData> OnInventorySlotSelected;

    private void Awake()
    {
        
    }

    public void SetItemData(InventoryListItemData data)
    {
        Data = data;
    }

    public void OnClick()
    {
        OnInventorySlotClicked?.Invoke(Data);
    }
    
    public void OnSelected()
    {
        transform.DOComplete();
        transform.DOPunchPosition(Vector3.down * 10f, 0.15f, 20, 1f);
        
        OnInventorySlotSelected?.Invoke(Data);
        GetComponentInChildren<Button>().Select();
    }
}
