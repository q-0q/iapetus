using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.Misc;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BellController : MonoBehaviour
{
    private Interactable _interactable;
    private Animator _animator;
    public string persistentEvent;
    private bool rung;
    public Transform bellMesh;
    private Material _bellMaterial;
    private CinemachineVirtualCamera _virtualCamera;
    private Transform _virtualCameraLookAtTarget;
    private Transform _ambientParticlesHolder;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        var saveData = SaveSystem.LoadSaveData(0);
        rung = saveData.persistentEvents.Contains(persistentEvent);
        _bellMaterial = bellMesh.GetComponent<Renderer>().material;

    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        Util.ReplaceAnimatorTrigger(_animator, "Idle");
        _virtualCamera.transform.SetParent(null);
        _virtualCameraLookAtTarget = transform.Find("VirtualCameraLookAtTarget");
        _ambientParticlesHolder = transform.Find("AmbientParticlesHolder");
        _virtualCameraLookAtTarget.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        _ambientParticlesHolder.Rotate(new Vector3(0, Time.deltaTime * 50f, 0));
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    private void OnInteracted()
    {
        // if (rung) return;
        
        Util.ReplaceAnimatorTrigger(_animator, "Ring");
        rung = true;

        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            CutsceneManager.Singleton.SetPseudoCutsceneActive();
            _virtualCamera.Priority = 20;
            yield return new WaitForSeconds(2.25f);
            transform.DOShakePosition(3f, 0.2f, 30);
            Util.InvokeSphereEffect(transform.position + (Vector3.down * 2f), Vector3.one * 17f, 1.35f, 1f, -5f);

            Vector3 virtualCameraLookAtStartPosition = _virtualCameraLookAtTarget.position;

            float t = 0;
            float duration = 1f;
            while (t < duration)
            {
                var value = Mathf.Lerp(0f, 1f, (duration - t) / duration);
                _bellMaterial.SetFloat("_GlowWeight", value);
                t += Time.deltaTime;
                _virtualCameraLookAtTarget.position = virtualCameraLookAtStartPosition + Vector3.down * (value * 2f);
                yield return null;
            }

            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _virtualCameraLookAtTarget.position = virtualCameraLookAtStartPosition;
            _virtualCamera.Priority = -10;
        }
        
    }
}
