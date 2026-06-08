using System;
using System.Collections;
using Code.Misc;
using DG.Tweening;
using UnityEngine;

public class PlayerDeathParticles : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private float _baseDot;
    private float _baseFresnel;
    private Vector3 _baseScale;
    private Renderer _renderer;
    private Material _material;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _renderer = transform.Find("Halo").GetComponent<Renderer>();
        _baseScale = _renderer.transform.localScale;
        _material = _renderer.material;
        _baseDot = _material.GetFloat("_Dot");
        _renderer.enabled = false;
    }

    public void PlayDeath()
    {
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            transform.position = PlayerFsm.Singleton.transform.position;
            var t = 0f;
            var d = 0.4f;

            yield return new WaitForSeconds(0.05f);
            
            _renderer.transform.localScale = _baseScale;
            _renderer.enabled = true;
            _renderer.transform.DOShakePosition(0.5f, 0.4f, 30);
            _material.SetFloat("_Dot", _baseDot);
            
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _material.SetFloat("_Dot", Mathf.Lerp(_material.GetFloat("_Dot"), 0, Time.deltaTime * 5f));
                _renderer.transform.localScale = Vector3.Lerp(_renderer.transform.localScale, _baseScale * 3f, Time.deltaTime * 12f);
                t += Time.deltaTime;
                yield return null;
            }

            _renderer.enabled = false;
            // yield return new WaitForSeconds(0.4f);
            _particleSystem.Play();
        }
    }

    public void PlayRespawn()
    {
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
