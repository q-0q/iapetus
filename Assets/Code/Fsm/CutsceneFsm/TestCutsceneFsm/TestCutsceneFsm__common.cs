using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
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
    private const string CutscenePersistentEvent = "IntroCutsceneCompleted";

    private Interactable _interactable;
    private ParticleSystem _interactableParticles;

    private Transform _endPosition;
    private Vector3 _stateGondolaStartingPosition;

    private Vector3 _interactablePosA;
    private Vector3 _interactablePosB;
    private Vector3 _interactableParticlesPosA;
    private Vector3 _interactableParticlesPosB;

    public EventReference musicEventReference;
    public EventReference windEventReference;
    
    public EventReference gondolaCreakEventReference;
    public EventReference gondolaMinorBangEventReference;
    public EventReference gondolaBreakEventReference;
    public EventReference gondolaCrashEventReference;
    private EventInstance _creakEventInstance;

    private int _currentTextId;
    private List<string> texts = new List<string>()
    {
        "With the passing of each age, the world summit's hearth is lulled from flame into smolder and smolder into cold ash.",
        "As the flames die, dark frost from beneath the world takes hold of the mountain's holiest places.",
        "The encroaching winter rimes the passage of time itself, freezing deeper until only howling wind moves on the mountain.",
        "As the braziers of the summit flame lay bare, the world falls dormant, perhaps never to wake again..."
    };

    private const float CanvasFadeDuration = 5f;
    private CinemachineVirtualCamera _finalVirtualCamera;
    private float _initialFogEndDistance;
    private float _initialFogStartDistance;

    private bool _waitingToSpawnBackgroundElement;

    private IEnumerator SpawnBackgroundElementCoroutine()
    {
        if (_waitingToSpawnBackgroundElement) yield break; 
        _waitingToSpawnBackgroundElement = true;
        yield return new WaitForSeconds(Random.Range(1.5f, 3.0f));
        var prefab = Resources.Load("Prefab/GondolaBackgroundRock") as GameObject;
        var forwardOffset = Vector3.forward * 70f;
        var sideOffset = (Random.Range(-1f, 1f) > 0 ? Vector3.left : Vector3.right) * Random.Range(40f, 50f);
        var obj = Instantiate(prefab, transform.position + forwardOffset + sideOffset, Quaternion.Euler(0f, Random.Range(0, 360f), 0), null);
        var s = Random.Range(0.5f, 1.5f);
        obj.transform.localScale = new Vector3(s, s, s);
        _waitingToSpawnBackgroundElement = false;
        StartCoroutine(DestroyObjectAfterDuration(obj, 25f));
    }

    private IEnumerator DestroyObjectAfterDuration(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(obj);
    }
}