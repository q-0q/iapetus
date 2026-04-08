using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using Util = Code.Misc.Util;

public class KeyItemRegistration
{
    public string displayName;
    public string metaName;
    public GameObject MeshGameObject;
    public Action onUse;
}

public static class KeyItemRegistry
{
    public static readonly Dictionary<string, KeyItemRegistration> KeyItemRegistrations;

    static KeyItemRegistry()
    {
        KeyItemRegistrations = new Dictionary<string, KeyItemRegistration>();
        
        KeyItemRegistrations.Add("UrnFragment1", new KeyItemRegistration()
        {
            displayName = "Urn Fragment",
            metaName = "urn-fragment-1",
            MeshGameObject = Resources.Load("Prefab/KeyItems/UrnFragment") as GameObject,
            onUse = null
        });
        
        KeyItemRegistrations.Add("UrnFragment2", new KeyItemRegistration()
        {
            displayName = "Urn Fragment",
            metaName = "urn-fragment-2",
            MeshGameObject = Resources.Load("Prefab/KeyItems/UrnFragment") as GameObject,
            onUse = null
        });
        
        KeyItemRegistrations.Add("UrnFragment3", new KeyItemRegistration()
        {
            displayName = "Urn Fragment",
            metaName = "urn-fragment-3",
            MeshGameObject = Resources.Load("Prefab/KeyItems/UrnFragment") as GameObject,
            onUse = null
        });
    }
}


public class KeyItem : MonoBehaviour
{

    public string Id;
    private Interactable _interactable;
    private Transform _meshTransform;
    private bool collected = false;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var data = KeyItemRegistry.KeyItemRegistrations[Id];
        _meshTransform = Instantiate(data.MeshGameObject, transform).transform;
    }

    private void OnInteracted()
    {
        
        if (collected) return;
        collected = true;
        _interactable.SetEnabled(false);
        
        StartCoroutine(PositionCoroutine());
        StartCoroutine(RotationCoroutine());
        StartCoroutine(ScaleCoroutine());
        
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.KeyItemCollect);
        
        IEnumerator PositionCoroutine()
        {
            var t = 0f;
            var d = 0.5f;

            var start = transform.position;
            var end = PlayerFsm.Singleton.transform.position + Vector3.up * 6.5f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                transform.position = Util.LerpWithArc(start, end, w, 1f);
                _meshTransform.localPosition = Vector3.Lerp(_meshTransform.localPosition, Vector3.zero, w);
                _particleSystem.transform.position = _meshTransform.position;
                t += Time.deltaTime;
                yield return null;
            }

            _particleSystem.Stop();
            yield return new WaitForSeconds(0.7f);
            
            t = 0f;
            d = 0.5f;
            
            start = transform.position;
            end = PlayerFsm.Singleton.transform.position + Vector3.up * 2.5f + PlayerFsm.Singleton.transform.forward;
            
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                transform.position = Vector3.Lerp(start, end, w);
                t += Time.deltaTime;
                yield return null;
            }
        }
        
        IEnumerator ScaleCoroutine()
        {
            var t = 0f;
            var d = 0.95f;
            yield return new WaitForSeconds(1.3f);
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _meshTransform.localScale = Vector3.Lerp(_meshTransform.localScale, Vector3.zero, w);
                t += Time.deltaTime;
                yield return null;
            }
        }
        
        IEnumerator RotationCoroutine()
        {
            var t = 0f;
            var d = 1.5f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _meshTransform.Rotate(Vector3.up, Time.deltaTime * Mathf.Lerp(1000f, 0f, Mathf.Pow(w, 2f)));
                t += Time.deltaTime;
                yield return null;
            }

        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (collected) return;
        _meshTransform.Rotate(Vector3.up, Time.deltaTime * 130f);
        _meshTransform.localPosition = new Vector3(0, (Mathf.Sin(Time.time * 2f) + 1f) * 0.5f,0);
        _particleSystem.transform.position = _meshTransform.position;
    }
}
