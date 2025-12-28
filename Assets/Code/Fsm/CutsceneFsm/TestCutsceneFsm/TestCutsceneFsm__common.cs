using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class TestCutsceneFsm
{
    private PlayerInput _playerInput;
    // private Interactable _interactable;
    private CinemachineVirtualCamera _virtualCamera;
    private Transform _playerTransformOnStart;
    [SerializeField] private Transform gondola;
    [SerializeField] private Transform innerCube;
    private CanvasGroup _mainCanvasGroup;
    private CanvasGroup _textCanvasGroup;
    private TextMeshProUGUI _textTmp;
    private bool _moveCubeForwardShake1 = false;
    private bool _moveCubeForwardShake2 = false;
    private float _textClock;
    private ParticleSystem _impactParticles;

    private Transform _endPosition;
    private Vector3 _stateGondolaStartingPosition;
    
    private int _currentTextId;
    private List<string> texts = new List<string>()
    {
        "With the passing of each age, the world summit's hearth is lulled from flame into smolder and smolder into cold ash.",
        "As the flames die, dark frost from beneath the world takes hold of the mountain's holiest places.",
        "The encroaching winter rimes the passage of time itself, freezing deeper until only howling wind moves on the mountain.",
        "As the braziers of the summit flame lay bare, the world falls dormant, perhaps never to wake again..."
    };

    private float _moveCubeForwardDuration = 20f;
}