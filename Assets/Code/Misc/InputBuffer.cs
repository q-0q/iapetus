using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer
{
    private Dictionary<string, float> _buffer;
    private PlayerInput _playerInput;
    private float _windowSize;
    
    public InputBuffer(PlayerInput playerInput, float windowSize)
    {
        _playerInput = playerInput;
        _windowSize = windowSize;
        _buffer = new Dictionary<string, float>();
    }

    public void InitInput(string input)
    {
        _buffer.Add(input, _windowSize + 1f);
    }
    
    public bool IsBuffered(string input)
    {
        return _buffer[input] <= _windowSize;
    }
    
    public void ConsumeBuffer(string input)
    {
        _buffer[input] = _windowSize + 1f;
    }

    public void OnUpdate()
    {
        List<string> keys = new List<string>(_buffer.Keys);
        foreach (var input in keys)
        {
            _buffer[input] += Time.deltaTime;
            if (_playerInput.actions[input].WasPressedThisFrame())
            {
                _buffer[input] = 0;
            }
        }
    }

}