using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;


public class BreakableObject : MonoBehaviour
{
    private bool broken = false;
    private bool jiggling = false;
    private ParticleSystem _particleSystem;
    private MeshRenderer _meshRenderer;
    private EventReference _eventReference;
    private float _bitChance;
    
    public void Set(Mesh mesh, Material material, EventReference eventReference, float bitChance)
    {
        GetComponent<MeshFilter>().mesh = mesh;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = material;
        _eventReference = eventReference;
        transform.Find("Particles").TryGetComponent(out _particleSystem);
        _particleSystem.GetComponent<Renderer>().material = material;
        _bitChance = bitChance;

    }

    private void Update()
    {
        var distance = Vector3.Distance(PlayerFsm.Singleton.transform.position, transform.position);
        if (distance < 2f && !broken)
        {
            StartCoroutine(OnBreak());
        }
        else if (distance < 3f && !jiggling && PlayerFsm.Singleton.GetMomentum() > 6f)
        {
            StartCoroutine(OnJiggle());
        }
    }

    private void Start()
    {
        transform.Find("Particles").TryGetComponent(out _particleSystem);
    }

    IEnumerator OnBreak()
    {
        if (broken) yield break;
        broken = true;

        if (Random.Range(0f, 1f) < _bitChance) BitSystem.Singleton.SpawnFromPool(transform.position + Vector3.up * 2f);
        
        _meshRenderer.enabled = false;
        _particleSystem.transform.SetParent(null);
        _particleSystem.Play();
        FMODUnity.RuntimeManager.PlayOneShotAttached(_eventReference, gameObject);
        yield return new WaitForSeconds(1f);
        Destroy(_particleSystem.gameObject);
        Destroy(gameObject);
        yield break;
    }
    
    IEnumerator OnJiggle()
    {
        if (jiggling) yield break;
        jiggling = true;
        
        // transform.DOShakePosition(0.3f, 0.5f, 7);
        transform.DOPunchRotation((transform.position - PlayerFsm.Singleton.transform.position).normalized * 10.25f,
            0.7f, 5, 0.5f);
        yield return new WaitForSeconds(0.75f);
        
        jiggling = false;
        yield break;
    }
}
