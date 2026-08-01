using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{

    private TriggerProxy _triggerProxy;
    private float respawnTimer;
    private const float RespawnDuration = 2.5f;
    private Transform _fx;

    private void Awake()
    {
        _triggerProxy = GetComponentInChildren<TriggerProxy>();
        respawnTimer = RespawnDuration;
        _fx = transform.Find("Fx");
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
        if (RespawnDuration - respawnTimer < Time.deltaTime)
        {
            _fx.position = transform.position;
            _fx.localScale = Vector3.one;
        }
        
        respawnTimer += Time.deltaTime;
    }

    private void OnTriggerProxyStay(Collider obj)
    {
        if (respawnTimer > RespawnDuration)
        {
            respawnTimer = 0f;
            PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.SurgeDashStartup);
            StartCoroutine(Coroutine());
        }

        IEnumerator Coroutine()
        {
            var t = 0f;
            var d = 0.75f;
            while (t < d)
            {
                _fx.position = Vector3.Lerp(_fx.position, PlayerFsm.Singleton.transform.position +
                                                                    (Vector3.up * 1.5f), t / d);
                _fx.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / d);
                t += Time.unscaledDeltaTime;
                yield return null;
                
            }

            
        }
    }
}
