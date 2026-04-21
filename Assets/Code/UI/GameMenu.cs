using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static GameMenu Singleton;
    private PlayerInput _playerInput;
    private PlayerInput _defaultPlayerInput;
    private GameObject _menu;
    private CinemachineFreeLook _freeLook;
    private GameObject _settings;
    private GameObject _buttons;

    public static event Action OnGameMenuOpened;
    public static event Action OnGameMenuClosed;
    

    private void Awake()
    {
        Singleton = this;
    }
    

    private void Start()
    {
        transform.Find("DefaultPlayerInput").TryGetComponent(out _defaultPlayerInput);
        TryGetComponent(out _playerInput);
        _menu = transform.Find("Menu").gameObject;
        _settings = transform.Find("Menu").Find("SettingsMenu").gameObject;
        _buttons = transform.Find("Menu").Find("Buttons").gameObject;
        _freeLook = FindObjectOfType<CinemachineFreeLook>();
    }

    private void Update()
    {
        if (_playerInput.actions["Menu"].WasPressedThisFrame())
        {
            print("menu pressed");
            if (_menu.activeInHierarchy)
            {
                OnMenuClosed();
            }
            else
            {
                OnGameMenuOpened?.Invoke();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _menu.SetActive(true);
                // _menu.transform.Find("Buttons").Find("Continue").GetComponent<Button>().Select();
                Time.timeScale = 0.000001f;
            }
        }


        if (IsMenuOpen() && NeedToSelect())
        {
            _menu.transform.Find("Buttons").Find("Continue").GetComponent<Button>().Select();
        }

    }
    
    private bool NeedToSelect()
    {
        foreach (var selectable in GetComponentsInChildren<Selectable>())
        {
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject) return false;
        }

        return _defaultPlayerInput.actions["Navigate"].ReadValue<Vector2>().magnitude > 0.1f;
    }

    private void OnEnable()
    {
        SettingsMenu.OnSettingsMenuClosed += OnSettingsClosed;
    }
    
    private void OnDisable()
    {
        SettingsMenu.OnSettingsMenuClosed -= OnSettingsClosed;
    }

    private void OnSettingsClosed()
    {
        // _menu.transform.Find("Buttons").Find("Continue").GetComponent<Button>().Select();
        _buttons.SetActive(true);
        _settings.SetActive(false);
    }

    public void OnSettingsClicked()
    {
        _buttons.SetActive(false);
        _settings.SetActive(true);
    }

    public void OnMenuClosed()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _freeLook.m_RecenterToTargetHeading.CancelRecentering();
        _menu.SetActive(false);
        Time.timeScale = 1f;
        OnGameMenuClosed?.Invoke();
    }
    
    public void OnQuitToMain()
    {
        SaveSystem.WriteCachedSave();
        SceneLoader.Singleton.LoadScene("MainMenu");
    }

    public void OnMenuReset()
    {
        SceneLoader.Singleton.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsMenuOpen()
    {
        return _menu.activeInHierarchy;
    }
}
