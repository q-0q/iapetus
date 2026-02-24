using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class BitDeposit : MonoBehaviour
{

    public string metaName;
    public int bitCount = 20;
    private Renderer _renderer;
    private Material _material;
    private Collider _collider;
    public EventReference _breakEvent;
    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = transform.Find("Mesh").GetComponent<Renderer>();
        _material = _renderer.material;
        TryGetComponent(out _collider);

        if (SaveSystem.GetBitDeposit(metaName, 0))
        {
            _collider.enabled = false;
            _renderer.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(BreakCoroutine());
    }

    private IEnumerator BreakCoroutine()
    {
        for (int i = 0; i < bitCount; i++)
        {
            BitSystem.Singleton.SpawnFromPool(_renderer.transform.position + Vector3.up * 2f);
        }
        
        SaveSystem.CollectBitDeposit(metaName, 0);
        FMODUnity.RuntimeManager.PlayOneShotAttached(_breakEvent, gameObject);
        Util.InvokeSphereEffect(transform.position + (Vector3.up * 2f), Vector3.one * 15f, 1.35f, 1f, -5f);
        _renderer.transform.DOShakePosition(0.35f, 0.5f, 25);
        float t = 0;
        float duration = 0.4f;
        while (t < duration)
        {
            _material.SetFloat("_Break", t / duration);
            yield return null;
            t += Time.deltaTime;
        }
        
        _collider.enabled = false;
        _renderer.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
