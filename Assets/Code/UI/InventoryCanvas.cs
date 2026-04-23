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

    enum InventoryCanvasState
    {
        Closed,
        Main,
        UseConfirmation
    }

    private PlayerInput _playerInput;
    private InventoryCanvasState _state = InventoryCanvasState.Closed;
    private CanvasGroup _canvasGroup;
    private CanvasGroup _mainCanvasGroup;
    private CanvasGroup _useConfirmationCanvasGroup;
    private Image _closeImage;

    public static InventoryCanvas Singleton;

    private List<InventorySlot> _inventorySlots;

    private GameObject _selection;
    private TextMeshProUGUI _selectionName;
    private TextMeshProUGUI _selectionDescription;
    private TextMeshProUGUI _selectionUseDescription;
    
    private TextMeshProUGUI _useConfirmationItemName;
    private TextMeshProUGUI _useConfirmation;
    
    private const int MaximumSlotCount = 10;
    private KeyItemRegistration _confirmationData;
    
    private void Awake()
    {
        Singleton = this;
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _mainCanvasGroup = transform.Find("MainView").GetComponent<CanvasGroup>();
        _useConfirmationCanvasGroup = transform.Find("UseConfirmationView").GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
        _mainCanvasGroup.alpha = 0;
        _useConfirmationCanvasGroup.alpha = 0;
        
        _closeImage = _mainCanvasGroup.transform.Find("CloseInput").Find("Image").GetComponent<Image>();
        
        _selection = _mainCanvasGroup.transform.Find("Selection").gameObject;
        _selectionName = _mainCanvasGroup.transform.Find("Selection").Find("Name").GetComponent<TextMeshProUGUI>();
        _selectionDescription = _mainCanvasGroup.transform.Find("Selection").Find("Description").GetComponent<TextMeshProUGUI>();
        _selectionUseDescription = _mainCanvasGroup.transform.Find("Selection").Find("UseDescription").GetComponent<TextMeshProUGUI>();
        
        _useConfirmation = _useConfirmationCanvasGroup.transform.Find("Selection").Find("UseConfirmation").GetComponent<TextMeshProUGUI>();
        _useConfirmationItemName = _useConfirmationCanvasGroup.transform.Find("Selection").Find("Name").GetComponent<TextMeshProUGUI>();
        
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


        if (_playerInput.actions["Look"].ReadValue<Vector2>().magnitude > 0.01 &&
            InputTypeManager.Singleton.GetCurrentInputType() == InputTypeManager.InputType.Kmb && GetIsOpen())
        {
            Cursor.visible = true;  
            Cursor.lockState = CursorLockMode.None;
        }
        
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, GetIsOpen() ? 1f : 0f, Time.deltaTime * 20f);
        _closeImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Inventory");

        if (_state == InventoryCanvasState.Main)
        {
            _mainCanvasGroup.alpha = Mathf.Lerp(_mainCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
        if (_state == InventoryCanvasState.UseConfirmation)
        {
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _mainCanvasGroup.alpha = Mathf.Lerp(_mainCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
        // if (NeedToSelect()) _inventorySlots[0].GetComponentInChildren<Button>().Select();
    }

    void Close()
    {
        _state = InventoryCanvasState.Closed;
        
        _canvasGroup.blocksRaycasts = false;
        _mainCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.blocksRaycasts = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Open()
    {
        _state = InventoryCanvasState.Main;
        _canvasGroup.blocksRaycasts = true;
        _mainCanvasGroup.blocksRaycasts = true;
        _useConfirmationCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.alpha = 0;
        
        _inventorySlots[0].GetComponentInChildren<Button>().Select();
        OnSlotSelected(_inventorySlots[0].Data);
        
        if (TutorialCanvas.Singleton.GetCurrentAction() == "Inventory") TutorialCanvas.Singleton.HideTutorialText();
    }

    void ConfirmationViewOpen()
    {
        _state = InventoryCanvasState.UseConfirmation;
        _mainCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.blocksRaycasts = true;
        _useConfirmationCanvasGroup.transform.Find("Selection").Find("Buttons").Find("Back").GetComponent<Button>().Select();
    }
    
    public void ConfirmationViewClose()
    {
        _state = InventoryCanvasState.Main;
        _inventorySlots[0].GetComponentInChildren<Button>().Select();
        OnSlotSelected(_inventorySlots[0].Data);
        
        _mainCanvasGroup.blocksRaycasts = true;
        _useConfirmationCanvasGroup.blocksRaycasts = false;
    }
    
    public void ConfirmationViewConfirm()
    {
        _confirmationData.onUse?.Invoke();
    }

    public bool GetIsOpen()
    {
        return _state != InventoryCanvasState.Closed;
    }
    
    private bool NeedToSelect()
    {
        foreach (var selectable in GetComponentsInChildren<Selectable>())
        {
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject && _selection.activeSelf) return false;
        }

        return true;
    }

    public void SelectionClicked()
    {
        OnSlotClicked(_confirmationData);
    }


    private void OnSlotClicked(KeyItemRegistration data)
    {
        if (!data.GetCanUse())
        {
            _selectionUseDescription.transform.DOComplete();
            _selectionUseDescription.transform.DOPunchPosition(Vector3.right * 10f, 0.25f, 30, 1f);
            return;
        }
        
        ConfirmationViewOpen();
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

        _useConfirmationItemName.text = data.displayName;
        _useConfirmation.text = data.GetUseConfirmation();
        _confirmationData = data;
    }

    private void OnGameMenuOpened()
    {
        if (!GetIsOpen()) return;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
    
    private void OnGameMenuClosed()
    {
        if (!GetIsOpen()) return;
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }
    
    
}
