using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using Util = Code.Misc.Util;

public class Lemon : MonoBehaviour
{

    public static event Action OnLemonCollected;
    private Animator _animator;
    private ParticleSystem _readyParticles;
    private Material _material;
    private Renderer _renderer;
    private Transform _bone;

    public string MetaName;

    private EventInstance _passiveInstance;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        transform.Find("ReadyParticles").TryGetComponent(out _readyParticles);
        _renderer = transform.Find("Mesh").Find("lemon").Find("Lemon").GetComponent<Renderer>();
        _material = _renderer.material;
        _bone = transform.Find("Mesh").Find("lemon").Find("Armature").Find("Bone");
        
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 10f, Fsm.GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
        }

        if (SaveSystem.GetLemonCollection(MetaName))
        {
            _renderer.enabled = false;
            _readyParticles.Stop();
            _readyParticles.Clear();
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            _passiveInstance.start();
        }
        
        
        Util.ReplaceAnimatorTrigger(_animator, "Ready");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        OnCollected();
    }

    // Update is called once per frame
    void Update()
    {
        // _readyParticles.transform.position = _bone.position;
    }

    private void OnCollected()
    {
        StartCoroutine(TimescaleCoroutine());
        Util.ReplaceAnimatorTrigger(_animator, "Collected");
        _readyParticles.Stop();
        _readyParticles.Clear();
        
        var main = _readyParticles.main;
        main.simulationSpeed = 2.75f;
        GetComponent<Collider>().enabled = false;
        SaveSystem.WriteLemonCollection(MetaName);
        FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference("event:/LemonCollect"), gameObject);
        _passiveInstance.stop(STOP_MODE.ALLOWFADEOUT);
        Util.InvokeSphereEffect(transform.position + Vector3.up * 2.5f, Vector3.one * 6f, 1.25f, 0.8f, -1f);
        StartCoroutine(DoCollectionTintWeight());
        OnLemonCollected?.Invoke();
    }

    private IEnumerator DoCollectionTintWeight()
    {
        yield return new WaitForSeconds(0.25f);
        float t = 0;
        float duration = 0.25f;
        while (t < duration)
        {
            _material.SetFloat("_TintWeight", t / duration);
            yield return null;
            t += Time.deltaTime;
        }

        
        yield return new WaitForSeconds(0.5f);
        Util.InvokeSphereEffect(transform.position + Vector3.up * 5.5f, Vector3.one * 6f, 1.25f, 0.8f, -1f);
        // _renderer.enabled = false;
    }
    
    private IEnumerator TimescaleCoroutine()
    {

        yield return new WaitForSeconds(0.15f);
        
        float timescale = 0.15f;
        float t = 0;
        float duration = 0.1f;
        while (t < duration)
        {
            Time.timeScale = Mathf.Lerp(1f, timescale, Util.SmoothLerp01(t / duration));
            yield return null;
            t += Time.deltaTime;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        t = 0;
        duration = 0.25f;
        while (t < duration)
        {
            Time.timeScale = Mathf.Lerp(timescale, 1f, Util.SmoothLerp01(t / duration));
            yield return null;
            t += Time.deltaTime;
        }


        Time.timeScale = 1f;
        
    }

    private void Awake()
    {
        _passiveInstance =
            FMODUnity.RuntimeManager.CreateInstance(
                FMODUnity.RuntimeManager.PathToEventReference("event:/LemonPassive"));
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_passiveInstance, gameObject);
    }

    private void OnDisable()
    {
        _passiveInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
