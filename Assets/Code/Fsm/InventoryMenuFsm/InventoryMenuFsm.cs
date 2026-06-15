using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class InventoryMenuFsm : Fsm
{
    public class InventoryMenuFsmState : FsmState
    {
        public static int Closed;
        public static int Open;
        public static int Bag;
        public static int UseConfirmation;
        public static int Movelist;
    }

    public class InventoryMenuFsmTrigger : FsmTrigger
    {
        public static int Closed;
        public static int Opened;
        public static int Bag;
        public static int Movelist;
        public static int Use;
        public static int Confirm;
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        
        Singleton = this;
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _listCanvasGroup = transform.Find("ListView").GetComponent<CanvasGroup>();
        _useConfirmationCanvasGroup = transform.Find("UseConfirmationView").GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
        _listCanvasGroup.alpha = 0;
        _useConfirmationCanvasGroup.alpha = 0;
        
        _closeImage = transform.Find("CloseInput").Find("Image").GetComponent<Image>();
        
        _listSelection = _listCanvasGroup.transform.Find("Selection").gameObject;
        _listSelectionName = _listCanvasGroup.transform.Find("Selection").Find("Name").GetComponent<TextMeshProUGUI>();
        _listSelectionDescription = _listCanvasGroup.transform.Find("Selection").Find("Description").GetComponent<TextMeshProUGUI>();
        _listSelectionUseDescription = _listCanvasGroup.transform.Find("Selection").Find("UseDescription").GetComponent<TextMeshProUGUI>();
        
        _useConfirmation = _useConfirmationCanvasGroup.transform.Find("Selection").Find("UseConfirmation").GetComponent<TextMeshProUGUI>();
        _useConfirmationItemName = _useConfirmationCanvasGroup.transform.Find("Selection").Find("Name").GetComponent<TextMeshProUGUI>();
        
        _listSelection.SetActive(false);
        
        
        TryGetComponent(out _playerInput);
        
        
        
        
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = InventoryMenuFsmState.Closed;
        PopulateBagData(SaveSystem.LoadCachedSaveData());
        
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        

        
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, Machine.IsInState(InventoryMenuFsmState.Closed) ? 0f : 1f, Time.deltaTime * 20f);
        _closeImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Inventory");

        if (Machine.IsInState(InventoryMenuFsmState.Open))
        {
            if (_playerInput.actions["Look"].ReadValue<Vector2>().magnitude > 0.01 &&
                InputTypeManager.Singleton.GetCurrentInputType() == InputTypeManager.InputType.Kmb)
            {
                Cursor.visible = true;  
                Cursor.lockState = CursorLockMode.None;
            }
            
            if (_playerInput.actions["Move"].ReadValue<Vector2>().magnitude > 0.01)
            {
                Cursor.visible = false;  
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        if (Machine.IsInState(InventoryMenuFsmState.Bag))
        {
            _listCanvasGroup.alpha = Mathf.Lerp(_listCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
        if (Machine.IsInState(InventoryMenuFsmState.UseConfirmation))
        {
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _listCanvasGroup.alpha = Mathf.Lerp(_listCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
    }

    private void OnEnable()
    {
        PlayerFsm.PlayerInventoryEntered += Open;
        PlayerFsm.PlayerInventoryExited += Close;
        SaveSystem.OnSaveDataUpdated += PopulateBagData;
        // InventorySlot.OnInventorySlotClicked += OnSlotClicked;
        InventoryListItem.OnInventorySlotSelected += OnListItemSelected;
        InventoryListItem.OnInventoryListItemUsed += OnListItemUsed;
        GameMenu.OnGameMenuOpened += OnGameMenuOpened;
        GameMenu.OnGameMenuClosed += OnGameMenuClosed;
        
    }

    private void OnDisable()
    {
        PlayerFsm.PlayerInventoryEntered -= Open;
        PlayerFsm.PlayerInventoryExited -= Close;
        SaveSystem.OnSaveDataUpdated -= PopulateBagData;
        // InventorySlot.OnInventorySlotClicked -= OnSlotClicked;
        InventoryListItem.OnInventorySlotSelected -= OnListItemSelected;
        InventoryListItem.OnInventoryListItemUsed -= OnListItemUsed;
        GameMenu.OnGameMenuOpened -= OnGameMenuOpened;
        GameMenu.OnGameMenuClosed -= OnGameMenuClosed;
    }
}
