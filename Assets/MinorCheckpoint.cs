using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinorCheckpoint : MonoBehaviour
{

    private static MinorCheckpoint _currentMinorCheckpoint;
    private ParticleSystem _activeParticles;
    private ParticleSystem _triggerParticles;
    private ParticleSystem _seekParticles;
    private Transform _playerSpawnTransform;
    private Light _light;


    private static event Action<MinorCheckpoint> OnPlayerMinorCheckpointSet;

    private void OnPlayerMinorCheckpointSetMethod(MinorCheckpoint minorCheckpoint)
    {
        
        // print(minorCheckpoint.name);
        // return;
        
        if (minorCheckpoint == this)
        {
            if (_currentMinorCheckpoint == this) return;
            _currentMinorCheckpoint = this;
            StartCoroutine(InvokeSeekParticles());
        }
        else
        {
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
        _playerSpawnTransform = transform.Find("PlayerSpawnTransform");
        _light = GetComponentInChildren<Light>();
        _currentMinorCheckpoint = null;
    }

    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(PlayerFsm.Singleton.transform.position, transform.position);
        if (distance < 10f)
        {
            OnPlayerMinorCheckpointSet?.Invoke(this);
        }
        
    }

    IEnumerator InvokeSeekParticles()
    {
        _seekParticles.Play();
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
        
        _seekParticles.Stop();
        SaveSystem.WritePlayerInGamePosition(new[]
            {
                _playerSpawnTransform.position.x,
                _playerSpawnTransform.position.y,
                _playerSpawnTransform.position.z
            },
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
