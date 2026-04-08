using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryCanvas : MonoBehaviour
{

    private PlayerInput _playerInput;
    private bool _open = false;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput.actions["Inventory"].WasPressedThisFrame())
        {
            if (_open) Close();
            else Open();
        };
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _open ? 1f : 0f, Time.deltaTime * 20f);
    }

    void Close()
    {
        _open = false;
    }

    void Open()
    {
        _open = true;
        if (TutorialCanvas.Singleton.GetCurrentAction() == "Inventory") TutorialCanvas.Singleton.HideTutorialText();
    }
}
