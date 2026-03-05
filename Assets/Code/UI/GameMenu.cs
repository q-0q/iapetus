using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public static GameMenu Singleton;
    private PlayerInput _playerInput;
    private GameObject _menu;
    private CinemachineFreeLook _freeLook;
    private GameObject _settings;
    private GameObject _buttons;

    private void Awake()
    {
        Singleton = this;
    }

    public bool IsMenuOpen()
    {
        return _menu.activeInHierarchy;
    }

    private void Start()
    {
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
            if (_menu.activeInHierarchy)
            {
                OnMenuClosed();
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                _menu.SetActive(true);
                _menu.transform.Find("Buttons").Find("Continue").GetComponent<Button>().Select();
                Time.timeScale = 0.000001f;
            }
        }
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
        _menu.transform.Find("Buttons").Find("Continue").GetComponent<Button>().Select();
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
    }
    
    public void OnQuitToMain()
    {
        SaveSystem.WriteCachedSave(0);
        SceneLoader.Singleton.LoadScene("MainMenu");
    }

    public void OnMenuReset()
    {
        SceneLoader.Singleton.LoadScene(SceneManager.GetActiveScene().name);
    }
}
