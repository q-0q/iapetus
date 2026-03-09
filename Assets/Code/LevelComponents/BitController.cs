using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

public class BitController : MonoBehaviour
{
    private float timer = 0;
    private float _lifetime;
    private Transform _mesh;
    private Vector3 initialPosition;
    private Vector3 _knockDirection;
    private Material _material;
    private ParticleSystem _enableParticles;
    public static event Action OnBitCountUpdated;
    public EventReference _collectionEvent;
    
    // Start is called before the first frame update
    void Awake()
    {
        _mesh = transform.Find("Mesh");
        _material = GetComponentInChildren<Renderer>().material;
        transform.Find("EnableParticles").TryGetComponent(out _enableParticles);
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, initialPosition + Vector3.up * 1.5f, Time.deltaTime * 10f);
        transform.position = Vector3.Lerp(transform.position, PlayerFsm.Singleton.transform.position + Vector3.up * 0.5f,
            Time.deltaTime * 25f * Mathf.InverseLerp(0.5f, _lifetime, timer));
        
        transform.position = Vector3.Lerp(transform.position, initialPosition + _knockDirection * 5f,
            Time.deltaTime * 12f * Mathf.InverseLerp(_lifetime, 0.25f, timer));

        var color = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(0.5f, _lifetime, timer));
        
        _material.SetColor("_TopAdd", color);
        _material.SetColor("_SidesAdd", color);
        
        var scale = Mathf.Lerp(1f, 0.5f, Mathf.InverseLerp(0.5f, _lifetime, timer));
        transform.localScale = Vector3.one * scale;
        
        if (timer >= _lifetime)
        {
            Util.InvokeSphereEffect(transform.position - Vector3.up, Vector3.one * Random.Range(0.75f, 1.25f), 1.25f, 1f, -2f);
            SaveSystem.AddBit();
            OnBitCountUpdated?.Invoke();
            FMODUnity.RuntimeManager.PlayOneShotAttached(_collectionEvent, PlayerFsm.Singleton.gameObject);
            BitSystem.Singleton.ReturnObject(gameObject);
        }
    }

    private void OnEnable()
    {
        timer = 0;
        _lifetime = Random.Range(0.9f, 1.3f);
        initialPosition = transform.position;
        var unitCircle = Random.insideUnitCircle.normalized;
        _knockDirection = new Vector3(unitCircle.x, 0f, unitCircle.y);
        _knockDirection *= Random.Range(0.5f, 1.5f);
        _mesh.DOShakePosition(_lifetime, 0.5f, 8);
        transform.localScale = Vector3.one;
        _material.SetColor("_TopAdd", Color.black);
        _material.SetColor("_SidesAdd", Color.black);
        _enableParticles.Play();
        
    }
}
