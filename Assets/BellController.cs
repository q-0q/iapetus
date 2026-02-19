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
    [FormerlySerializedAs("persistentEvent")] public string metaName;
    private bool rung;
    public Transform bellMesh;
    private Material _bellMaterial;
    private Material _haloMaterial;
    private CinemachineVirtualCamera _virtualCamera;
    private Transform _virtualCameraLookAtTarget;
    public Transform armHolder;
    private Vector3 _startPosition;
    public static event Action OnBellRing;
    
    public static event Action OnPlayerNearbyRungBell;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        rung = SaveSystem.GetBell(metaName);
        _bellMaterial = bellMesh.GetComponent<Renderer>().material;
        _haloMaterial = transform.Find("Halo").GetComponent<Renderer>().material;
        if (!rung) return;
        _bellMaterial.SetFloat("_GlowWeight", 1f);
        _bellMaterial.SetFloat("_NoiseWeight", 0f);
        _haloMaterial.SetFloat("_Weight", 0f);
        armHolder.gameObject.SetActive(false);
        _interactable.SetEnabled(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position;
        _animator = GetComponentInChildren<Animator>();
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        Util.ReplaceAnimatorTrigger(_animator, "Idle");
        _virtualCamera.transform.SetParent(null);
        _virtualCameraLookAtTarget = transform.Find("VirtualCameraLookAtTarget");
        _virtualCameraLookAtTarget.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _startPosition + Vector3.up * (Mathf.Sin(Time.time * 1f) * 0.4f);
        if (rung && Vector3.Distance(transform.position, PlayerFsm.Singleton.transform.position) < 25f)
        {
            OnPlayerNearbyRungBell?.Invoke();
        }
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
        
        _interactable.SetEnabled(false);
        
        
        SaveSystem.WriteBell(metaName, 0);
        
        Util.ReplaceAnimatorTrigger(_animator, "Ring");

        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            CutsceneManager.Singleton.SetPseudoCutsceneActive();
            _virtualCamera.Priority = 20;
            
            float t = 0;
            float duration = 2.25f;
            while (t < duration)
            {
                var value = Mathf.Lerp(0f, 1f, (duration - t) / duration);
                
                _haloMaterial.SetFloat("_Weight", value);
                t += Time.deltaTime;
                yield return null;
            }

            rung = true;
            transform.DOShakePosition(3f, 0.2f, 30);
            Util.InvokeSphereEffect(transform.position + (Vector3.down * 2f), Vector3.one * 17f, 1.35f, 1f, -5f);
            _bellMaterial.SetFloat("_GlowWeight", 1f);
            armHolder.gameObject.SetActive(false);
            _interactable.SetEnabled(false);
            OnBellRing?.Invoke();
            
            Vector3 virtualCameraLookAtStartPosition = _virtualCameraLookAtTarget.position;

            t = 0;
            duration = 1f;
            while (t < duration)
            {
                var value = Mathf.Lerp(0f, 1f, (duration - t) / duration);
                
                t += Time.deltaTime;
                _virtualCameraLookAtTarget.position = virtualCameraLookAtStartPosition + Vector3.down * (value * 2f);
                yield return null;
            }

            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _virtualCameraLookAtTarget.position = virtualCameraLookAtStartPosition;
            _virtualCamera.Priority = -10;
            
            duration = 1f;
            t = 0;
            while (t < duration)
            {
                var value = Mathf.Lerp(0f, 1f, (duration - t) / duration);
                _bellMaterial.SetFloat("_NoiseWeight", value);

                _bellMaterial.SetFloat("_FadeWeight", value);
                t += Time.deltaTime;
                yield return null;
            }
        }
        
    }
}
