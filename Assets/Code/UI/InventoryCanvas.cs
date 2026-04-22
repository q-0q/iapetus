using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using TMPro;
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

    private GameObject _selection;
    private TextMeshProUGUI _selectionName;
    private TextMeshProUGUI _selectionDescription;
    private TextMeshProUGUI _selectionUseDescription;

    private const int MaximumSlotCount = 10;
    
    private void Awake()
    {
        Singleton = this;
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _closeImage = transform.Find("CloseInput").Find("Image").GetComponent<Image>();
        
        _selection = transform.Find("Selection").gameObject;
        _selectionName = transform.Find("Selection").Find("Name").GetComponent<TextMeshProUGUI>();
        _selectionDescription = transform.Find("Selection").Find("Description").GetComponent<TextMeshProUGUI>();
        _selectionUseDescription = transform.Find("Selection").Find("UseDescription").GetComponent<TextMeshProUGUI>();
        
        _selection.SetActive(false);

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
        InventorySlot.OnInventorySlotClicked += OnSlotClicked;
        InventorySlot.OnInventorySlotSelected += OnSlotSelected;
        GameMenu.OnGameMenuOpened += OnGameMenuOpened;
        GameMenu.OnGameMenuClosed += OnGameMenuClosed;
    }

    private void OnDisable()
    {
        PlayerFsm.PlayerInventoryEntered -= Open;
        PlayerFsm.PlayerInventoryExited -= Close;
        SaveSystem.OnSaveDataUpdated -= UpdateInventorySlots;
        InventorySlot.OnInventorySlotClicked -= OnSlotClicked;
        InventorySlot.OnInventorySlotSelected -= OnSlotSelected;
        GameMenu.OnGameMenuOpened -= OnGameMenuOpened;
        GameMenu.OnGameMenuClosed -= OnGameMenuClosed;
    }

    // Update is called once per frame
    void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _open ? 1f : 0f, Time.deltaTime * 20f);
        _closeImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Inventory");

        if (_playerInput.actions["Look"].ReadValue<Vector2>().magnitude > 0.01 &&
            InputTypeManager.Singleton.GetCurrentInputType() == InputTypeManager.InputType.Kmb && _open)
        {
            Cursor.visible = true;  
            Cursor.lockState = CursorLockMode.None;
        }
        
        // if (NeedToSelect()) _inventorySlots[0].GetComponentInChildren<Button>().Select();
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
        
        _inventorySlots[0].GetComponentInChildren<Button>().Select();
        OnSlotSelected(_inventorySlots[0].Data);
        
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
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject && _selection.activeSelf) return false;
        }

        return true;
    }


    private void OnSlotClicked(KeyItemRegistration data)
    {
        print("clicked " + data.displayName);
    }
    
    private void OnSlotSelected(KeyItemRegistration data)
    {
        if (data == null)
        {
            _selection.SetActive(false);
            return;
        }
        _selection.SetActive(true);
        _selection.transform.DOComplete();
        _selection.transform.DOPunchRotation(Vector3.forward * 2f, 0.15f, 20, 1f);
        
        _selectionName.text = data.displayName;
        _selectionDescription.text = data.description;
        _selectionUseDescription.text = data.GetUseDescription();
    }

    private void OnGameMenuOpened()
    {
        if (!_open) return;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
    
    private void OnGameMenuClosed()
    {
        if (!_open) return;
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }
    
    
}
