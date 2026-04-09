using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image _image;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _image = transform.Find("Rotator").Find("Button").Find("Image").GetComponent<Image>();
    }

    public void SetItemData(KeyItemRegistration data)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(true);
        _image.sprite = data.Sprite;
        transform.Find("Rotator").transform.rotation = Quaternion.identity;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
