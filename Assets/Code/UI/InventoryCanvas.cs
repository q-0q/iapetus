using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{

    enum InventoryCanvasState
    {
        Closed,
        Bag,
        Movelist,
        Test,
        UseConfirmation
    }
    
    private PlayerInput _playerInput;
    private InventoryCanvasState _state = InventoryCanvasState.Closed;
    private CanvasGroup _canvasGroup;
    private CanvasGroup _bagCanvasGroup;
    private CanvasGroup _useConfirmationCanvasGroup;
    private Image _closeImage;

    private static InventoryCanvas Singleton;

    private List<InventorySlot> _inventorySlots;

    private GameObject _bagSelection;
    private TextMeshProUGUI _bagSelectionName;
    private TextMeshProUGUI _bagSelectionDescription;
    private TextMeshProUGUI _bagSelectionUseDescription;
    
    private TextMeshProUGUI _bagUseConfirmationItemName;
    private TextMeshProUGUI _bagUseConfirmation;
    
    private const int MaximumSlotCount = 10;
    private KeyItemRegistration _confirmationData;



    public Button navBagButton;
    public Button navMovelistButton;
    public Button navTestButton;

    private List<Button> navButtons;
    
    
    
    private void Awake()
    {
        Singleton = this;
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _bagCanvasGroup = transform.Find("BagView").GetComponent<CanvasGroup>();
        _useConfirmationCanvasGroup = transform.Find("UseConfirmationView").GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
        _bagCanvasGroup.alpha = 0;
        _useConfirmationCanvasGroup.alpha = 0;
        
        _closeImage = transform.Find("CloseInput").Find("Image").GetComponent<Image>();
        
        _bagSelection = _bagCanvasGroup.transform.Find("Selection").gameObject;
        _bagSelectionName = _bagCanvasGroup.transform.Find("Selection").Find("Name").GetComponent<TextMeshProUGUI>();
        _bagSelectionDescription = _bagCanvasGroup.transform.Find("Selection").Find("Description").GetComponent<TextMeshProUGUI>();
        _bagSelectionUseDescription = _bagCanvasGroup.transform.Find("Selection").Find("UseDescription").GetComponent<TextMeshProUGUI>();
        
        _bagUseConfirmation = _useConfirmationCanvasGroup.transform.Find("Selection").Find("UseConfirmation").GetComponent<TextMeshProUGUI>();
        _bagUseConfirmationItemName = _useConfirmationCanvasGroup.transform.Find("Selection").Find("Name").GetComponent<TextMeshProUGUI>();
        
        _bagSelection.SetActive(false);

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

        if (_state == InventoryCanvasState.Bag)
        {
            _bagCanvasGroup.alpha = Mathf.Lerp(_bagCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
        if (_state == InventoryCanvasState.UseConfirmation)
        {
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _bagCanvasGroup.alpha = Mathf.Lerp(_bagCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
        // if (NeedToSelect()) _inventorySlots[0].GetComponentInChildren<Button>().Select();
    }

    void Close()
    {
        _state = InventoryCanvasState.Closed;
        
        _canvasGroup.blocksRaycasts = false;
        _bagCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.blocksRaycasts = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Open()
    {
        _state = InventoryCanvasState.Bag;
        _canvasGroup.blocksRaycasts = true;
        _bagCanvasGroup.blocksRaycasts = true;
        _useConfirmationCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.alpha = 0;
        
        _inventorySlots[0].GetComponentInChildren<Button>().Select();
        OnSlotSelected(_inventorySlots[0].Data);
        
        if (TutorialCanvas.Singleton.GetCurrentAction() == "Inventory") TutorialCanvas.Singleton.HideTutorialText();
    }

    void ConfirmationViewOpen()
    {
        _state = InventoryCanvasState.UseConfirmation;
        _bagCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.blocksRaycasts = true;
        _useConfirmationCanvasGroup.transform.Find("Selection").Find("Buttons").Find("Back").GetComponent<Button>().Select();
    }
    
    public void ConfirmationViewClose()
    {
        _state = InventoryCanvasState.Bag;
        _inventorySlots[0].GetComponentInChildren<Button>().Select();
        OnSlotSelected(_inventorySlots[0].Data);
        
        _bagCanvasGroup.blocksRaycasts = true;
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
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject && _bagSelection.activeSelf) return false;
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
            _bagSelectionUseDescription.transform.DOComplete();
            _bagSelectionUseDescription.transform.DOPunchPosition(Vector3.right * 10f, 0.25f, 30, 1f);
            return;
        }
        
        ConfirmationViewOpen();
    }
    
    private void OnSlotSelected(KeyItemRegistration data)
    {
        if (data == null)
        {
            _bagSelection.SetActive(false);
            return;
        }
        _bagSelection.SetActive(true);
        _bagSelection.transform.DOComplete();
        _bagSelection.transform.DOPunchRotation(Vector3.forward * 2f, 0.15f, 20, 1f);
        
        _bagSelectionName.text = data.displayName;
        _bagSelectionDescription.text = data.description;
        _bagSelectionUseDescription.text = data.GetUseDescription();

        _bagUseConfirmationItemName.text = data.displayName;
        _bagUseConfirmation.text = data.GetUseConfirmation();
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
