using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using Code.Misc;
using UnityEditor.Rendering;
using UnityEngine.Serialization;
using Util = Code.Misc.Util;

public class TravelController : MonoBehaviour
{

    public string id = "CHANGE ME";
    public string destinationId = "CHANGE ME";
    
    private Animator _animator;

    private const string gateEventSuffix = "-travel-gate";
    public Interactable GateInteractable;
    public Interactable MainInteractable;
    public Transform parentTransform;
    public Transform walkToPositionTransform;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        GateInteractable.OnInteracted += OnGateInteracted;
        MainInteractable.OnInteracted += OnMainInteracted;
    }

    private void OnMainInteracted()
    {
        IEnumerator CameraOffsetCoroutine()
        {
            var t = 0f;
            var d = 2.5f;
            var freelook = PlayerCinemachineFreeLook.Singleton;
            var offset = Vector3.zero;

            yield return new WaitForSeconds(0.5f);
            
            while (t < d)
            {
                var speed = Mathf.Lerp(0.1f, 2f, Mathf.InverseLerp(0f, 0.5f, t));
                offset = Vector3.Lerp(offset, new Vector3(0, 0, -12f), Time.deltaTime * speed);
                freelook.SetDesiredOffset(offset);
                t += Time.deltaTime;
                yield return null;
            }
        }
        
        IEnumerator Coroutine()
        {
            PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.WalkToTravelPosition);
            PlayerFsm.Singleton.walkToPositionTarget = walkToPositionTransform.position;
            PlayerFsm.Singleton.walkToPositionArrivalDistanceModifier = 1f;

            var d = 5f;
            var t = 0f;
            while(!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Travel))
            {
                t += Time.deltaTime;
                if (t > d) yield break;
                yield return null;
            }
            
            PlayerFsm.Singleton.MakeAllRenderersInvisible();
            PlayerFsm.Singleton.SetParentTransform(parentTransform);
            Util.ReplaceAnimatorTrigger(_animator, "MainChannel");

            StartCoroutine(CameraOffsetCoroutine());
            
            yield return new WaitForSeconds(2.15f);


            t = 0;
            d = 0.25f;

            while (t < d)
            {
                var speed = Mathf.Lerp(0f, 100f, Mathf.InverseLerp(0, 0.25f, t));
                parentTransform.position += -transform.right * (Time.deltaTime * speed);
                t += Time.deltaTime;
                yield return null;
            }

            var destinationRegistration = TravelRegistry[destinationId];
            SaveSystem.WritePlayerInGamePosition(Vector3.zero, destinationRegistration.PositionIdName, 0);
            SceneLoader.Singleton.LoadScene(destinationRegistration.HostSceneName);
            
            t = 0;
            d = 1f;

            while (t < d)
            {
                parentTransform.position += -transform.right * (Time.deltaTime * 100f);
                t += Time.deltaTime;
                yield return null;
            }

        }

        StartCoroutine(Coroutine());
    }

    private void OnDisable()
    {
        GateInteractable.OnInteracted -= OnGateInteracted;
        MainInteractable.OnInteracted -= OnMainInteracted;
    }

    private void OnGateInteracted()
    {
        Util.ReplaceAnimatorTrigger(_animator, "GateOpening");
        SaveSystem.WritePersistentEvent(id + gateEventSuffix);
        GateInteractable.SetEnabled(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SaveSystem.GetPersistentEventCompleted(id + gateEventSuffix))
        {
            Util.ReplaceAnimatorTrigger(_animator, "GateOpen");
            GateInteractable.SetEnabled(false);
        }
        
        // Util.ReplaceAnimatorTrigger(_animator, "MainOpen");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Util.ReplaceAnimatorTrigger(_animator, "GateOpen");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Util.ReplaceAnimatorTrigger(_animator, "MainChannel");
        }
    }

    public class TravelRegistration
    {
        public string DisplayName;
        public string HostSceneName;
        public string PositionIdName;
    }

    public static readonly Dictionary<string, TravelRegistration> TravelRegistry = new()
    {
        {
            "glyph", new TravelRegistration()
            {
                DisplayName = "Relay Hub",
                HostSceneName = "03-IcyCanals",
                PositionIdName = "travel",
            }
        },
        
        {
        "ouro", new TravelRegistration()
        {
            DisplayName = "Ouro Station",
            HostSceneName = "05-DaisClimb",
            PositionIdName = "travel",
        }
    }
    };
}

