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
    private const string CutscenePersistentEvent = "IntroCutsceneCompleted";
    public static event Action<Vector3> OnIntroCutsceneGondolaTeleported;
    public static event Action OnChannelStarted;
    private Transform _backgroundParent;

    private Interactable _interactable;
    private ParticleSystem _interactableParticles;
    private Material _particlesHaloMaterial;

    [SerializeField] private Transform _endPosition;
    private Vector3 _stateGondolaStartingPosition;

    private Vector3 _interactablePosA;
    private Vector3 _interactablePosB;
    private Vector3 _interactableParticlesPosA;
    private Vector3 _interactableParticlesPosB;

    [SerializeField] private Transform armVibrator;

    [SerializeField] private LineRenderer _lineRenderer;
    
    public EventReference musicEventReference;
    public EventReference windEventReference;
    
    public EventReference gondolaCreakEventReference;
    public EventReference gondolaMinorBangEventReference;
    public EventReference gondolaBreakEventReference;
    public EventReference gondolaCrashEventReference;
    public EventReference gondolaGroanEventReference;
    private EventInstance _creakEventInstance;

    public EventReference gondolaInteractEventReference;
    public EventReference gondolaInteractReadyEventReference;
    public EventReference gondolaInteractChannelEventReference;

    private string _currentNeededTriggerId;

    private SplineContainer _minorSpline;

    private CinemachineVirtualCamera _channelCamera;
    private SplineContainer _channelSpline;
    private CinemachineTrackedDolly _channelDolly;

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
    private CinemachineVirtualCamera _finalVirtualCamera;
    private float _initialFogEndDistance;
    private float _initialFogStartDistance;

    private bool _waitingToSpawnBackgroundElement;
    private Transform _particlesHalo;

    private IEnumerator SpawnBackgroundElementCoroutine()
    {
        if (_waitingToSpawnBackgroundElement) yield break; 
        _waitingToSpawnBackgroundElement = true;
        var prefab = Resources.Load("Prefab/GondolaBackgroundRock") as GameObject;
        var forwardOffset = Vector3.forward * 70f;
        var sideOffset = (Random.Range(-1f, 1f) > 0 ? Vector3.left : Vector3.right) * Random.Range(40f, 50f);
        var obj = Instantiate(prefab, transform.position + forwardOffset + sideOffset, Quaternion.Euler(0f, Random.Range(0, 360f), 0), _backgroundParent);
        var s = Random.Range(0.5f, 1.5f);
        obj.transform.localScale = new Vector3(s, s, s);
        StartCoroutine(DestroyObjectAfterDuration(obj, 25f));
        yield return new WaitForSeconds(Random.Range(1.5f, 3.0f));
        _waitingToSpawnBackgroundElement = false;
    }

    private IEnumerator DestroyObjectAfterDuration(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (obj == null) yield break;
        Destroy(obj);
    }

    private void OnTrigger(string id)
    {
        if (_currentNeededTriggerId == "a" && id == "a") _currentNeededTriggerId = "b";
        if (_currentNeededTriggerId == "b" && id == "b") Machine.Fire(TestCutsceneFsmTrigger.OnTriggersCompleted);
    }
}