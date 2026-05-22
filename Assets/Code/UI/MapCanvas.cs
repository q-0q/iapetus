using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapCanvas : MonoBehaviour
{

    public static MapCanvas Singleton;
    private PlayerInput _playerInput;
    private CanvasGroup _canvasGroup;
    private CanvasGroup _mainCanvasGroup;
    private CanvasGroup _useConfirmationCanvasGroup;
    private Image _closeImage;
    private MapCanvasState _state;
    
    enum MapCanvasState
    {
        Closed,
        Main,
        UseConfirmation
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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
    }

    private void OnEnable()
    {
        PlayerFsm.PlayerMapEntered += Open;
        PlayerFsm.PlayerMapExited += Close;
        GameMenu.OnGameMenuOpened += OnGameMenuOpened;
        GameMenu.OnGameMenuClosed += OnGameMenuClosed;
    }

    private void OnDisable()
    {
        PlayerFsm.PlayerMapEntered -= Open;
        PlayerFsm.PlayerMapExited -= Close;
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
        // _closeImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Map");

        if (_state == MapCanvasState.Main)
        {
            _mainCanvasGroup.alpha = Mathf.Lerp(_mainCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
        if (_state == MapCanvasState.UseConfirmation)
        {
            _useConfirmationCanvasGroup.alpha = Mathf.Lerp(_useConfirmationCanvasGroup.alpha, 1f, Time.deltaTime * 20f);
            _mainCanvasGroup.alpha = Mathf.Lerp(_mainCanvasGroup.alpha, 0f, Time.deltaTime * 20f);
        }
        
    }
    
    void Open()
    {
        _state = MapCanvasState.Main;
        _canvasGroup.blocksRaycasts = true;
        _mainCanvasGroup.blocksRaycasts = true;
        _useConfirmationCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.alpha = 0;
        
        if (TutorialCanvas.Singleton.GetCurrentAction() == "Map") TutorialCanvas.Singleton.HideTutorialText();
    }
    
    void Close()
    {
        _state = MapCanvasState.Closed;
        
        _canvasGroup.blocksRaycasts = false;
        _mainCanvasGroup.blocksRaycasts = false;
        _useConfirmationCanvasGroup.blocksRaycasts = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        EventSystem.current.SetSelectedGameObject(null);
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
    
    public bool GetIsOpen()
    {
        return _state != MapCanvasState.Closed;
    }
}
