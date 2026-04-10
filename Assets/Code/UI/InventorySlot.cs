using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image _image;
    private CanvasGroup _canvasGroup;
    public KeyItemRegistration Data;

    public static event Action<KeyItemRegistration> OnInventorySlotClicked;
    public static event Action<KeyItemRegistration> OnInventorySlotSelected;

    private void Awake()
    {
        _image = transform.Find("Rotator").Find("Button").Find("Image").GetComponent<Image>();
    }

    public void SetItemData(KeyItemRegistration data)
    {
        Data = data;
        
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        _image.sprite = data.Sprite;
        transform.Find("Rotator").transform.rotation = Quaternion.identity;
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
