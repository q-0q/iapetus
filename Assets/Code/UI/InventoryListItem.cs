using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private Transform _visualsTransform;
    private TextMeshProUGUI _mainTmp;

    private GameObject lastSelected;
    

    private void Awake()
    {
        _visualsTransform = GetComponentInChildren<Button>().transform.Find("Visuals");
        GetComponentInChildren<Button>().targetGraphic = _visualsTransform.Find("Panel").GetComponent<Image>();
        _mainTmp = _visualsTransform.Find("MainText").GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        var proxy = GetComponentInChildren<InventoryListItemButtonUIProxy>();
        proxy.OnMakeSelected += MakeSelected;
        proxy.OnMakeDeselected += MakeDeselected;
    }
    
    private void OnDisable()
    {
        var proxy = GetComponentInChildren<InventoryListItemButtonUIProxy>();
        proxy.OnMakeSelected -= MakeSelected;
        proxy.OnMakeDeselected -= MakeDeselected;
    }

    public void SetItemData(InventoryListItemData data)
    {
        Data = data;
        _mainTmp.text = data.displayName;
    }
    
    public void MakeSelected()
    {
        if (lastSelected == gameObject) return;
        lastSelected = gameObject;
        
        _visualsTransform.DOComplete();
        _visualsTransform.DOPunchPosition(Vector3.down * 10f, 0.15f, 20, 1f);
        
        OnInventorySlotSelected?.Invoke(Data);
        GetComponentInChildren<Button>().Select();
        _mainTmp.color = Color.black;
    }

    public void MakeDeselected()
    {
        lastSelected = EventSystem.current.currentSelectedGameObject;
        _mainTmp.color = Color.white;
    }
    
    
}
