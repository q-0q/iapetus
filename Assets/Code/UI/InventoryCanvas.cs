using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{

    private PlayerInput _playerInput;
    private bool _open = false;
    private CanvasGroup _canvasGroup;
    private Image _closeImage;

    public static InventoryCanvas Singleton;

    private List<InventorySlot> _inventorySlots;

    private const int MaximumSlotCount = 10;
    
    private void Awake()
    {
        Singleton = this;
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _closeImage = transform.Find("CloseInput").Find("Image").GetComponent<Image>();

        _inventorySlots = new List<InventorySlot>();
        foreach (var slot in GetComponentsInChildren<InventorySlot>())
        {
            _inventorySlots.Add(slot);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateInventorySlots(SaveSystem.LoadCachedSaveData());
    }

    void UpdateInventorySlots(SaveSystem.SaveData saveData)
    {
        foreach (var slot in _inventorySlots)
        {
            slot.SetItemData(null);
        }

        var items = saveData.items;
        for (int i = 0; i < items.Count; i++)
        {
            _inventorySlots[i].SetItemData(KeyItemRegistry.KeyItemRegistrations[items[i]]);
        }
    }

    private void OnEnable()
    {
        PlayerFsm.PlayerInventoryEntered += Open;
        PlayerFsm.PlayerInventoryExited += Close;
        SaveSystem.OnSaveDataUpdated += UpdateInventorySlots;
    }

    private void OnDisable()
    {
        PlayerFsm.PlayerInventoryEntered -= Open;
        PlayerFsm.PlayerInventoryExited -= Close;
        SaveSystem.OnSaveDataUpdated -= UpdateInventorySlots;
    }

    // Update is called once per frame
    void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _open ? 1f : 0f, Time.deltaTime * 20f);
        _closeImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Inventory");
        
        if (NeedToSelect()) _inventorySlots[0].GetComponentInChildren<Button>().Select();
    }

    void Close()
    {
        _open = false;
        
        _canvasGroup.blocksRaycasts = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Open()
    {
        _open = true;
        _canvasGroup.blocksRaycasts = true;

        if (InputTypeManager.Singleton.GetCurrentInputType() == InputTypeManager.InputType.Kmb)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        if (TutorialCanvas.Singleton.GetCurrentAction() == "Inventory") TutorialCanvas.Singleton.HideTutorialText();
    }

    public bool GetIsOpen()
    {
        return _open;
    }
    
    private bool NeedToSelect()
    {
        foreach (var selectable in GetComponentsInChildren<Selectable>())
        {
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject) return false;
        }

        return true;
    }
}
