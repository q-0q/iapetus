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
        public string confirmation;
        public bool canUse;
    }
    
    private CanvasGroup _canvasGroup;
    private InventoryListItemData data;

    public static event Action<InventoryListItemData> OnInventorySlotClicked;
    public static event Action<InventoryListItemData> OnInventorySlotSelected;
    public static event Action<InventoryListItemData> OnInventoryListItemUsed;

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
        proxy.OnUsed += OnUsed;
    }

    private void OnUsed()
    {
        OnInventoryListItemUsed?.Invoke(data);
    }

    private void OnDisable()
    {
        var proxy = GetComponentInChildren<InventoryListItemButtonUIProxy>();
        proxy.OnMakeSelected -= MakeSelected;
        proxy.OnMakeDeselected -= MakeDeselected;
        proxy.OnUsed -= OnUsed;
    }

    public void SetItemData(InventoryListItemData data)
    {
        this.data = data;
        _mainTmp.text = data.displayName;
    }
    
    public void MakeSelected()
    {
        if (lastSelected == gameObject) return;
        lastSelected = gameObject;
        
        _visualsTransform.DOComplete();
        _visualsTransform.DOPunchPosition(Vector3.down * 10f, 0.15f, 20, 1f);
        
        OnInventorySlotSelected?.Invoke(data);
        GetComponentInChildren<Button>().Select();
        _mainTmp.color = Color.black;
    }

    public void MakeDeselected()
    {
        lastSelected = EventSystem.current.currentSelectedGameObject;
        _mainTmp.color = Color.white;
    }
    
    
}
