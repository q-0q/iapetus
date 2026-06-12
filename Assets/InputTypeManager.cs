using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTypeManager : MonoBehaviour
{

    public Sprite kmbMove;
    public Sprite kmbLook;
    public Sprite kmbJump;
    public Sprite kmbSprint;
    public Sprite kmbInteract;
    public Sprite kmbInventory;
    public Sprite kmbMap;
    public Sprite kmbTrick;

    public Sprite padMove;
    public Sprite padLook;
    public Sprite padJump;
    public Sprite padSprint;
    public Sprite padInteract;
    public Sprite padInventory;
    public Sprite padMap;
    public Sprite padTrick;
    
    public static InputTypeManager Singleton;
    private PlayerInput _playerInput;

    private InputType _currentInputType = InputType.Kmb;
    public enum InputType
    {
        Kmb,
        Pad
    }
    private void Awake()
    {
        Singleton = this;
        TryGetComponent(out _playerInput);
    }
    
    void Update()
    {
        if (_playerInput.actions["KmbButton"].WasPressedThisFrame())
        {
            _currentInputType = InputType.Kmb;
        }
        
        if (_playerInput.actions["KmbValue"].WasPressedThisFrame())
        {
            _currentInputType = InputType.Kmb;
        }
        
        if (_playerInput.actions["PadButton"].WasPressedThisFrame())
        {
            _currentInputType = InputType.Pad;
        }
        
        if (_playerInput.actions["PadValue"].WasPressedThisFrame())
        {
            _currentInputType = InputType.Pad;
        }
    }

    public Sprite GetSpriteForAction(string action)
    {
        if (_currentInputType == InputType.Kmb)
        {
            if (action == "Move") return kmbMove;
            if (action == "Look") return kmbLook;
            if (action == "Jump") return kmbJump;
            if (action == "Sprint") return kmbSprint;
            if (action == "Interact") return kmbInteract;
            if (action == "Inventory") return kmbInventory;
            if (action == "Map") return kmbMap;
            if (action == "Trick") return kmbTrick;
        }
        
        if (action == "Move") return padMove;
        if (action == "Look") return padLook;
        if (action == "Jump") return padJump;
        if (action == "Sprint") return padSprint;
        if (action == "Interact") return padInteract;
        if (action == "Inventory") return padInventory;
        if (action == "Map") return padMap;
        if (action == "Trick") return padTrick;

        return null;
    }

    public InputType GetCurrentInputType()
    {
        return _currentInputType;
    }

}
