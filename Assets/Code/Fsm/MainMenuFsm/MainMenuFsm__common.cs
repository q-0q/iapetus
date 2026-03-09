using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class MainMenuFsm
{
    private GameObject _homeObject;
    private GameObject _optionsObject;
    private GameObject _savesObject;
    private GameObject _chaptersObject;
    private GameObject _backButtonObject;
    private GameObject _newGameObject;

    private PlayerInput _playerInput;

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    private bool NeedToSelect(GameObject currentObject)
    {
        foreach (var selectable in GetComponentsInChildren<Selectable>())
        {
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject) return false;
        }

        return _playerInput.actions["Navigate"].ReadValue<Vector2>().magnitude > 0.1f;
    }
}