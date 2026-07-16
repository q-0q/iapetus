using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class IntroCutsceneFsm
{
    private PlayerInput _playerInput;
    // private Interactable _interactable;
    private CinemachineVirtualCamera _virtualCamera;
    private CanvasGroup _mainCanvasGroup;
    private CanvasGroup _textCanvasGroup;
    private TextMeshProUGUI _textTmp;
    private float _textClock;
    private const string CutscenePersistentEvent = "IntroCutsceneCompleted";

    public static event Action<Vector3> OnIntroCutsceneWarp;
    private ParticleSystem _particleSystem;
    private Renderer _curvedStarRenderer;
    private Renderer _haloRenderer;
    private Light _light;

    // private Interactable _interactable;




    
    public EventReference musicEventReference;
    public EventReference windEventReference;
    
    public Image textAdvanceImage;
    private int _currentTextId;
    private List<string> texts = new List<string>()
    {
        "With the passing of each age, the world summit's hearth is lulled from flame into smolder and smolder into cold ash.",
        "As the flames die, dark frost from beneath the world takes hold of the mountain's holiest places.",
        "The encroaching winter rimes the passage of time itself, freezing deeper until only howling wind moves on the mountain.",
        "As the braziers of the summit flame lay bare, the world falls dormant, perhaps never to wake again..."
    };

    private const float CanvasFadeDuration = 7f;
    
    
}