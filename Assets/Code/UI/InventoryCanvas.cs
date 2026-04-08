using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
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

    private const int MaximumSlots = 10;
    
    private void Awake()
    {
        Singleton = this;
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _closeImage = transform.Find("CloseInput").Find("Image").GetComponent<Image>();

        _inventorySlots = new List<InventorySlot>();
        var _slotTemplate = GetComponentInChildren<InventorySlot>();
        _inventorySlots.Add(_slotTemplate);
        
        

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        PlayerFsm.PlayerInventoryEntered += Open;
        PlayerFsm.PlayerInventoryExited += Close;
    }

    private void OnDisable()
    {
        PlayerFsm.PlayerInventoryEntered -= Open;
        PlayerFsm.PlayerInventoryExited -= Close;
    }

    // Update is called once per frame
    void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _open ? 1f : 0f, Time.deltaTime * 20f);
        _closeImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Inventory");
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
}
