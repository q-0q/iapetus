using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MinorCheckpoint : MonoBehaviour
{

    private static MinorCheckpoint _currentMinorCheckpoint;
    private ParticleSystem _activeParticles;
    private ParticleSystem _triggerParticles;
    private ParticleSystem _seekParticles;
    private Transform _playerSpawnTransform;
    private Light _light;
    private Renderer _haloRenderer;


    private static event Action<MinorCheckpoint> OnPlayerMinorCheckpointSet;
    
    public EventReference triggerEventReference;
    public EventReference seekEventReference;

    private void OnPlayerMinorCheckpointSetMethod(MinorCheckpoint minorCheckpoint)
    {
        
        // print(minorCheckpoint.name);
        // return;
        
        if (minorCheckpoint == this)
        {
            _haloRenderer.enabled = false;
            if (_currentMinorCheckpoint == this) return;
            _currentMinorCheckpoint = this;
            StartCoroutine(InvokeSeekParticles());
        }
        else
        {
            _haloRenderer.enabled = true;
            _light.enabled = false;
            _triggerParticles.Stop();
            _activeParticles.Stop();
        }
        
    }

    private void OnEnable()
    {
        OnPlayerMinorCheckpointSet += OnPlayerMinorCheckpointSetMethod;
    }

    private void OnDisable()
    {
        OnPlayerMinorCheckpointSet -= OnPlayerMinorCheckpointSetMethod;
    }


    // Start is called before the first frame update
    void Start()
    {
        transform.Find("TriggerParticles").TryGetComponent(out _triggerParticles);
        transform.Find("ActiveParticles").TryGetComponent(out _activeParticles);
        transform.Find("SeekParticles").TryGetComponent(out _seekParticles);
        _haloRenderer = transform.Find("Halo").GetComponent<Renderer>();
        _playerSpawnTransform = transform.Find("PlayerSpawnTransform");
        _light = GetComponentInChildren<Light>();
        _currentMinorCheckpoint = null;
    }

    // Update is called once per frame
    void Update()
    {
        var playerDistance = Vector3.Distance(PlayerFsm.Singleton.transform.position, transform.position);
        if (playerDistance < 20f)
        {
            OnPlayerMinorCheckpointSet?.Invoke(this);
        }
        else if (playerDistance > 80f && _currentMinorCheckpoint == this)
        {
            _haloRenderer.enabled = true;
            _light.enabled = false;
            _triggerParticles.Stop();
            _activeParticles.Stop();
            _currentMinorCheckpoint = null;
        }
        
    }

    IEnumerator InvokeSeekParticles()
    {
        _seekParticles.Play();
        Util.InvokeSphereEffect(transform.position, Vector3.one * 15f, 1.25f, 1f, -3f);
        FMODUnity.RuntimeManager.PlayOneShotAttached(seekEventReference, gameObject);
        var seekParticlesStartPosition = PlayerFsm.Singleton.transform.position + Vector3.up * 3f;
        var seekParticlesEndPosition = transform.position + Vector3.up * 0.1f;
        float t = 0f;
        var duration = 0.75f;
        while (t < duration)
        {
            var w = t / duration;
            _seekParticles.transform.position = LerpWithArc(seekParticlesStartPosition, seekParticlesEndPosition, w, 3f);
            t += Time.deltaTime;
            yield return null;
        }

        _triggerParticles.Play();
        _activeParticles.Play();
        _light.enabled = true;
        FMODUnity.RuntimeManager.PlayOneShotAttached(triggerEventReference, gameObject);
        _seekParticles.Stop();
        SaveSystem.WritePlayerInGamePosition(_playerSpawnTransform.position,
            "",
            _playerSpawnTransform.rotation.eulerAngles.y, 
            0);

    }
    
    public static Vector3 LerpWithArc(Vector3 start, Vector3 end, float t, float height)
    {
        // Clamp t for safety
        t = Mathf.Clamp01(t) * 0.9f;

        // Base linear interpolation
        Vector3 position = Vector3.Lerp(start, end, t);

        // Quadratic arc: peaks at t = 0.5, zero at t = 0 and t = 1
        float arc = 4f * height * t * (1f - t);

        // Apply arc in the world-up direction
        position += Vector3.up * arc;

        return position;
    }
}
