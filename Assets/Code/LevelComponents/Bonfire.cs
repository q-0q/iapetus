using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Bonfire : MonoBehaviour
{

    private static Bonfire _currentBonfire;
    private ParticleSystem _triggerParticles;
    private ParticleSystem _seekParticles;
    private Transform _playerSpawnTransform;
    private Light _light;
    private GameObject _whiteHalo;
    private string path;
    


    private static event Action<Bonfire> OnPlayerBonfireSet;
    
    public EventReference triggerEventReference;
    public EventReference seekEventReference;

    private Interactable _interactable;
    private GameObject _blackHalo;

    private void OnBonfireSetMethod(Bonfire bonfire)
    {
        
        if (bonfire == this)
        {
            _currentBonfire = this;
            StartCoroutine(InvokeSeekParticles());
        }
        else
        {
            _whiteHalo.SetActive(false);
            _triggerParticles.Stop();
            _light.enabled = false;
        }
        
    }

    private void OnEnable()
    {
        OnPlayerBonfireSet += OnBonfireSetMethod;
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnInteracted()
    {
        OnPlayerBonfireSet?.Invoke(this);
    }
    

    private void OnDisable()
    {
        OnPlayerBonfireSet -= OnBonfireSetMethod;
        _interactable.OnInteracted -= OnInteracted;
    }


    // Start is called before the first frame update
    void Awake()
    {
        transform.Find("TriggerParticles").TryGetComponent(out _triggerParticles);
        transform.Find("SeekParticles").TryGetComponent(out _seekParticles);
        _blackHalo = transform.Find("BlackHalo").gameObject;
        _whiteHalo = _blackHalo.transform.Find("WhiteHalo").gameObject;
        _playerSpawnTransform = transform.Find("PlayerSpawnTransform");
        _currentBonfire = null;
        _light = GetComponentInChildren<Light>();
        _light.enabled = false;
        _interactable = GetComponentInChildren<Interactable>();
        _whiteHalo.SetActive(false);
        path = transform.GetPath();

        if (SaveSystem.GetCheckpointPath() == path)
        {
            _currentBonfire = this;
            _whiteHalo.SetActive(true);
            _light.enabled = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        _blackHalo.SetActive(!CultTrialManager.Singleton.isCurseEnabled);
        _interactable.SetEnabled(!CultTrialManager.Singleton.isCurseEnabled);
        // var playerDistance = Vector3.Distance(PlayerFsm.Singleton.transform.position, transform.position);
        // var playerYDelta = PlayerFsm.Singleton.transform.position.y - transform.position.y;
        // if (playerDistance < 25f && playerYDelta > -5f)
        // {
        //     OnPlayerMinorCheckpointSet?.Invoke(this);
        // }
        // else if (playerDistance > 80f && _currentMinorCheckpoint == this)
        // {
        //     _triggerParticles.Stop();
        //     _currentMinorCheckpoint = null;
        // }
        
    }

    IEnumerator InvokeSeekParticles()
    {
        _seekParticles.Play();
        Util.InvokeSphereEffect(_whiteHalo.transform.position - Vector3.up, Vector3.one * 15f, 1.25f, 1f, -3f);
        FMODUnity.RuntimeManager.PlayOneShotAttached(seekEventReference, gameObject);
        _whiteHalo.SetActive(true);
        _light.enabled = true;
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
        FMODUnity.RuntimeManager.PlayOneShotAttached(triggerEventReference, gameObject);
        _seekParticles.Stop();
        SaveSystem.WriteCheckpointPath(path);
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
