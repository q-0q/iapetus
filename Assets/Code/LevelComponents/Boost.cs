using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{

    private TriggerProxy _triggerProxy;
    private float respawnTimer;
    private const float RespawnDuration = 2.5f;
    private Transform _center;
    private Transform _splines;
    private ParticleSystem _particles;
    private CustomPointLight _light;
    private Color _baseLightColor;

    public bool groundSnap = true;

    private void Awake()
    {
        _triggerProxy = GetComponentInChildren<TriggerProxy>();
        respawnTimer = RespawnDuration;
        _center = transform.Find("Fx").Find("Center");
        _splines = transform.Find("Fx").Find("Splines");
        _particles = GetComponentInChildren<ParticleSystem>();
        _light = GetComponentInChildren<CustomPointLight>();
        _baseLightColor = _light.Color;

    }

    private void Start()
    {
        if (groundSnap)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 25f,
                    Fsm.GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
            {
                transform.position = hit.point + Vector3.up * 5f;
            }
        }
    }

    private void OnEnable()
    {
        _triggerProxy.OnTriggerProxyStay += OnTriggerProxyStay;
    }

    private void OnDisable()
    {
        _triggerProxy.OnTriggerProxyStay -= OnTriggerProxyStay;
    }

    private void Update()
    {
        
        _splines.localRotation *= Quaternion.Euler(0f, 0f, Time.deltaTime * 50f);

        var d = RespawnDuration - respawnTimer;
        if (d < Time.deltaTime && d > 0)
        {
            _splines.gameObject.SetActive(true);
            _center.position = transform.position;
            _center.localScale = Vector3.one;
            _particles.Play();
            _light.Color = _baseLightColor;

        }
        
        respawnTimer += Time.deltaTime;
    }

    private void OnTriggerProxyStay(Collider obj)
    {
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Respawn)) return;
        if (PlayerFsm.Singleton.GetTimeSinceRespawn() < 2f) return;
        if (respawnTimer > RespawnDuration)
        {
            respawnTimer = 0f;
            
            StartCoroutine(Coroutine());

        }

        IEnumerator Coroutine()
        {
            var t = 0f;
            var d = 0.3f;
            while (t < d)
            {
                _center.position = Vector3.Lerp(transform.position, PlayerFsm.Singleton.transform.position +
                                                                    (Vector3.up * 1.5f), t / d);
                _center.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / d);
                t += Time.unscaledDeltaTime;
                yield return null;
                _light.Color = _baseLightColor * Mathf.Clamp01((1f - ((t * 2f) / d)));
            }
            
            _light.Color = Color.black;
            
            _particles.Stop();
            _splines.gameObject.SetActive(false);
            PlayerFsm.Singleton.QueueSurge();
            
            
        }
    }
}
