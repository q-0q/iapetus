using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMOD.Studio;
using UnityEngine;
using Util = Code.Misc.Util;

public class BellDoorController : MonoBehaviour
{

    private Animator _animator;
    private CinemachineVirtualCamera _passiveCamera;
    public static event Action OnPlayerNearbyUnopenedBellDoor;
    public string persistentEvent;
    
    private float _playerLookAwayTimer;
    private const float LookawayTime = 0.75f;
    public int bellRequirement = 2;

    private List<GameObject> _lightObjects;

    private const float lightPlacementRadius = 15f;
    private const float lightPlacementSeparationAngle = 25f;

    private const string _readyEventPath = "event:/BellDoorReady";
    private EventInstance _readyInstance;
    private Interactable _interactable;

    private void Awake()
    {
        _playerLookAwayTimer = 0f;
        _animator = GetComponentInChildren<Animator>();
        _passiveCamera = transform.Find("BellDoorPassiveCamera").GetComponentInChildren<CinemachineVirtualCamera>();
        _interactable = GetComponentInChildren<Interactable>();
        _interactable.SetEnabled(false);

        
        var lightPrefab = Resources.Load("Prefab/BellDoorLight") as GameObject;
        var lightHolder = transform.Find("LightHolder");
        _lightObjects = new List<GameObject>();
        
        for (int i = 0; i < bellRequirement; i++)
        {
            var angle = lightPlacementSeparationAngle * (i - (bellRequirement - 1) * 0.5f);
            var offset = Quaternion.Euler(angle, 0f, 0f) * Vector3.up * lightPlacementRadius;
            var lightObject = Instantiate(lightPrefab, lightHolder);
            lightObject.transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
            _lightObjects.Add(lightObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SaveSystem.GetPersistentEventCompleted(persistentEvent))
        {
            _readyInstance.stop(STOP_MODE.ALLOWFADEOUT);
            for (int i = 0; i < _lightObjects.Count; i++)
            {
                Util.InvokeSphereEffect(_lightObjects[i].transform.position - Vector3.up, Vector3.one * 8f, 1.25f, 1f, -3f);
                _lightObjects[i].transform.Find("Mesh").Find("Tetra").GetComponent<Renderer>().material.SetFloat("_Weight", 1);
                _lightObjects[i].transform.Find("Mesh").Find("Halo").GetComponent<Renderer>().material.SetFloat("_Weight", 0);
            }
        
            _interactable.SetEnabled(false);
            Util.ReplaceAnimatorTrigger(_animator, "Opened");
        }

        else
        {
            UpdatePlayerBellStatus();
        }

    }

    private void UpdatePlayerBellStatus()
    {
        var playerBellCount = SaveSystem.LoadSaveData(0).bellCount;
        for (int i = 0; i < _lightObjects.Count; i++)
        {
            var weight = i < playerBellCount ? 1f : 0f;
            _lightObjects[i].transform.Find("Mesh").Find("Tetra").GetComponent<Renderer>().material.SetFloat("_Weight", weight);
            _lightObjects[i].transform.Find("Mesh").Find("Halo").GetComponent<Renderer>().material.SetFloat("_Weight", weight);
        }

        if (playerBellCount >= bellRequirement)
        {
            _interactable.SetEnabled(true);
            _readyInstance.start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Angle(PlayerFsm.Singleton.transform.forward, transform.forward) < 90f)
        {
            _playerLookAwayTimer += Time.deltaTime;
        }
        else
        {
            _playerLookAwayTimer = 0;
        }
        
        if (_passiveCamera.Priority > 0) OnPlayerNearbyUnopenedBellDoor?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        _passiveCamera.Priority = _playerLookAwayTimer >= LookawayTime ? -20 : 20;
    }

    private void OnTriggerExit(Collider other)
    {
        _passiveCamera.Priority = -20;
    }

    private void OnInteracted()
    {
        _readyInstance.stop(STOP_MODE.ALLOWFADEOUT);
        for (int i = 0; i < _lightObjects.Count; i++)
        {
            _lightObjects[i].transform.Find("Mesh").Find("Halo").GetComponent<Renderer>().material.SetFloat("_Weight", 0);
            Util.InvokeSphereEffect(_lightObjects[i].transform.position - Vector3.up, Vector3.one * 8f, 1.25f, 1f, -3f);
        }
        
        _interactable.SetEnabled(false);
        Util.ReplaceAnimatorTrigger(_animator, "Open");
        SaveSystem.WritePersistentEvent(persistentEvent, 0);
        SaveSystem.ReduceBellCount(bellRequirement, 0);

    }

    private void OnEnable()
    {
        BellController.OnBellRing += UpdatePlayerBellStatus;
        _readyInstance = FMODUnity.RuntimeManager.CreateInstance(FMODUnity.RuntimeManager.PathToEventReference(_readyEventPath));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_readyInstance, _interactable.gameObject);
        _interactable.OnInteracted += OnInteracted;

    }
    
    private void OnDisable()
    {
        BellController.OnBellRing -= UpdatePlayerBellStatus;
        _interactable.OnInteracted -= OnInteracted;
        _readyInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
