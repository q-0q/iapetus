using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using DG.Tweening;
using UnityEngine;

public class Lemon : MonoBehaviour
{
    
    private Animator _animator;
    private ParticleSystem _readyParticles;
    private Material _material;
    private Renderer _renderer;
    private Transform _bone;

    public string MetaName;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        transform.Find("ReadyParticles").TryGetComponent(out _readyParticles);
        _renderer = transform.Find("Mesh").Find("lemon").Find("Lemon").GetComponent<Renderer>();
        _material = _renderer.material;
        _bone = transform.Find("Mesh").Find("lemon").Find("Armature").Find("Bone");
        
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 10f))
        {
            transform.position = hit.point;
        }

        if (SaveSystem.GetLemonCollection(MetaName, 0))
        {
            _renderer.enabled = false;
            _readyParticles.Stop();
            _readyParticles.Clear();
            GetComponent<Collider>().enabled = false;
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
        Util.ReplaceAnimatorTrigger(_animator, "Collected");
        _readyParticles.Stop();
        
        var main = _readyParticles.main;
        main.simulationSpeed = 2.75f;
        GetComponent<Collider>().enabled = false;
        SaveSystem.WriteLemonCollection(MetaName, 0);
        StartCoroutine(DoCollectionTintWeight());
    }

    private IEnumerator DoCollectionTintWeight()
    {
        yield return new WaitForSeconds(0.25f);
        float t = 0;
        float duration = 0.5f;
        while (t < duration)
        {
            _material.SetFloat("_TintWeight", t / duration);
            yield return null;
            t += Time.deltaTime;
        }

        
        yield return new WaitForSeconds(0.75f);
        Util.InvokeSphereEffect(transform.position + Vector3.up * 5.5f, Vector3.one * 6f, 1.25f, 0.8f, -1f);
        // _renderer.enabled = false;
    }
}
