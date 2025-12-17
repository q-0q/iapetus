using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class TestCutsceneFsm
{
    private PlayerInput _playerInput;
    private Interactable _interactable;
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private Transform cube;
    private CanvasGroup _canvasGroup;
}