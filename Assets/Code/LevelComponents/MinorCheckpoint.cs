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
    private ParticleSystem _triggerParticles;
    private ParticleSystem _seekParticles;
    private Transform _playerSpawnTransform;
    private Light _light;
    private GameObject _haloRenderer;

    private bool haloCoroutineActive = false;


    private static event Action<MinorCheckpoint> OnPlayerMinorCheckpointSet;
    
    public EventReference triggerEventReference;
    public EventReference seekEventReference;

    private void OnPlayerMinorCheckpointSetMethod(MinorCheckpoint minorCheckpoint)
    {
        
        if (minorCheckpoint == this)
        {
            if (_currentMinorCheckpoint == this) return;
            _currentMinorCheckpoint = this;
            StartCoroutine(InvokeSeekParticles());
        }
        else
        {
            _triggerParticles.Stop();
        }
        
    }

    private void OnEnable()
    {
        OnPlayerMinorCheckpointSet += OnPlayerMinorCheckpointSetMethod;
        _haloRenderer.SetActive(false);
        StartCoroutine(MakeHaloRendererEnabled());
    }

    private IEnumerator MakeHaloRendererEnabled()
    {
        haloCoroutineActive = true;
        yield return new WaitForSeconds(0.2f);
        _haloRenderer.SetActive(true);
        haloCoroutineActive = false;
    }

    private void OnDisable()
    {
        OnPlayerMinorCheckpointSet -= OnPlayerMinorCheckpointSetMethod;
        _haloRenderer.SetActive(false);
    }


    // Start is called before the first frame update
    void Awake()
    {
        transform.Find("TriggerParticles").TryGetComponent(out _triggerParticles);
        transform.Find("SeekParticles").TryGetComponent(out _seekParticles);
        _haloRenderer = transform.Find("Halo").gameObject;
        _playerSpawnTransform = transform.Find("PlayerSpawnTransform");
        _currentMinorCheckpoint = null;
        _light = GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!haloCoroutineActive) _haloRenderer.SetActive(!CultTrialManager.Singleton.isCurseEnabled);
        if (CultTrialManager.Singleton.isCurseEnabled) return;
        var playerDistance = Vector3.Distance(PlayerFsm.Singleton.transform.position, transform.position);
        var playerYDelta = PlayerFsm.Singleton.transform.position.y - transform.position.y;
        if (playerDistance < 25f && playerYDelta > -5f)
        {
            OnPlayerMinorCheckpointSet?.Invoke(this);
        }
        else if (playerDistance > 80f && _currentMinorCheckpoint == this)
        {
            _triggerParticles.Stop();
            _currentMinorCheckpoint = null;
        }
        
    }

    IEnumerator InvokeSeekParticles()
    {
        _seekParticles.Play();
        Util.InvokeSphereEffect(_haloRenderer.transform.position - Vector3.up, Vector3.one * 15f, 1.25f, 1f, -3f);
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

        // _triggerParticles.Play();
        // _light.enabled = true;
        FMODUnity.RuntimeManager.PlayOneShotAttached(triggerEventReference, gameObject);
        _seekParticles.Stop();
        SaveSystem.WritePlayerInGamePosition(_playerSpawnTransform.position,
            "",
            _playerSpawnTransform.rotation.eulerAngles.y);

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
