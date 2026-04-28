using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBuffer
{
    private Dictionary<string, float> _buffer;
    private Dictionary<string, bool> _negativeEdge;
    private Dictionary<string, bool> _cutsceneBuffer;
    private PlayerInput _playerInput;
    private float _windowSize;

    public InputBuffer(PlayerInput playerInput, float windowSize)
    {
        _playerInput = playerInput;
        _windowSize = windowSize;
        _cutsceneBuffer = new Dictionary<string, bool>();
        _negativeEdge = new Dictionary<string, bool>();
        _buffer = new Dictionary<string, float>();
    }

    public void InitInput(string input, bool negativeEdge = false, bool cutsceneEnabled = false)
    {
        _buffer.Add(input, _windowSize + 1f);
        _negativeEdge.Add(input, negativeEdge);
        _cutsceneBuffer.Add(input, cutsceneEnabled);
    }
    
    public bool IsBuffered(string input, float windowSizeOffset = 0f)
    {
        return _buffer[input] <= _windowSize + windowSizeOffset;
    }
    
    public void ConsumeBuffer(string input)
    {
        _buffer[input] = _windowSize + 1f;
    }

    public void OnUpdate(bool cutscenePlayerDisabled)
    {
        if (GameMenu.Singleton.IsMenuOpen()) return;
        List<string> keys = new List<string>(_buffer.Keys);
        foreach (var input in keys)
        {
            _buffer[input] += Time.deltaTime;
            if (cutscenePlayerDisabled && !_cutsceneBuffer[input]) continue;
            if (_playerInput.actions[input].WasPressedThisFrame())
            {
                _buffer[input] = 0;
            }
            if (_negativeEdge[input] && _playerInput.actions[input].WasReleasedThisFrame())
            {
                _buffer[input] = 0;
            }
        }
    }

}