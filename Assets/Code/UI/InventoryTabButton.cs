using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTabButton : MonoBehaviour
{

    public string label = "Label";

    public static event Action<string> OnInventoryTabButtonHovered;

    private Transform _visualsTransform;
    private TextMeshProUGUI _mainTmp;
    private Image _image;

    private GameObject lastSelected;
    

    private void Awake()
    {
        _visualsTransform = transform.Find("Visuals");
        _mainTmp = _visualsTransform.Find("Text").GetComponent<TextMeshProUGUI>();
        _mainTmp.text = label;
        _image = _visualsTransform.GetComponent<Image>();
    }

    private void OnEnable()
    {
        var proxy = GetComponentInChildren<InventoryListItemButtonUIProxy>();
        proxy.OnMakeSelected += MakeSelected;
        InventoryMenuFsm.OnInventoryTabSelected += OnHover;
    }

    private void OnHover(string obj)
    {
        if (label == obj)
        {
            _visualsTransform.DOComplete();
            _visualsTransform.DOPunchPosition(Vector3.down * 10f, 0.15f, 20, 1f);
            
            _mainTmp.color = Color.black;
            _visualsTransform.GetComponent<Image>().color = Color.white;
            return;
        };
        
        _mainTmp.color = Color.white;
        _visualsTransform.GetComponent<Image>().color = Color.black;
        
    }

    private void OnDisable()
    {
        var proxy = GetComponentInChildren<InventoryListItemButtonUIProxy>();
        proxy.OnMakeSelected -= MakeSelected;
    }
    
    
    public void MakeSelected()
    {
        OnInventoryTabButtonHovered?.Invoke(label);
    }
    
    
    
}
