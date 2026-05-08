using System;
using UnityEngine;

public class PlayerMinorLeylineHalo : MonoBehaviour
{
    private Renderer _renderer;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        Hide();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerFsm.Singleton.transform.position;
    }

    public void Show()
    {
        _particleSystem.Play();
        _renderer.enabled = true;
    }

    public void Hide()
    {
        _particleSystem.Stop();
        _renderer.enabled = false;
    }
}
