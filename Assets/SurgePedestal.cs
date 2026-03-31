using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using Code.TriggerParams;
using UnityEngine;

public class SurgePedestal : MonoBehaviour
{
    private Interactable _interactable;
    private Material _material;
    private Material _haloMaterial;

    private bool _isChanneling;

    private void Awake()
    {
        _isChanneling = false;
        _interactable = GetComponentInChildren<Interactable>();
        _material = GetComponent<Renderer>().material;
        _haloMaterial = transform.Find("Halo").GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (_isChanneling) return;
        
        _material.SetFloat("_Weight", Mathf.Lerp(_material.GetFloat("_Weight"), 0, Time.deltaTime * 10f));
        _haloMaterial.SetFloat("_Weight", Mathf.Lerp(_haloMaterial.GetFloat("_Weight"), 0, Time.deltaTime * 2f));
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
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.SurgePedestalInteracted, new SurgePedestalParam() { SurgePedestal = this });
    }

    private IEnumerator Coroutine()
    {
        _isChanneling = true;
        var t = 0f;
        var d = 1.5f;

        while (t < d)
        {
            var w = Util.SmoothLerp01(t / d);
            _material.SetFloat("_Weight", w);
            _haloMaterial.SetFloat("_Weight", w);
            t += Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    public void StartChannel()
    {
        if (_isChanneling) return;
        StartCoroutine(Coroutine());
    }

    public void EndChannel()
    {
        _isChanneling = false;
    }
}
