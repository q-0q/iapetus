using System;
using System.Collections;
using Cinemachine;
using Code.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

public class SentryProjectile : MonoBehaviour
{

    private const float Speed = 200f;
    private Vector3 _direction;
    private float _lifetime;

    private bool _impacted;
    private ParticleSystem _impactParticles;

    private CinemachineImpulseSource _impulse;
    private GameObject _deathCollider;
    
    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
        //
        // _direction = Vector3.RotateTowards(
        //     direction, 
        //     Random.onUnitSphere, 
        //     5f * Mathf.Deg2Rad, 
        //     0f
        // );
    }

    private void Awake()
    {
        _impacted = false;
        _lifetime = 0f;
        _impactParticles = transform.Find("Fx").Find("ImpactParticles").GetComponent<ParticleSystem>();
        _impulse = GetComponentInChildren<CinemachineImpulseSource>();
        _deathCollider = GetComponentInChildren<PlayerDeathCollider>().gameObject;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Util.InvokeSphereEffect(transform.position - Vector3.up, Vector3.one * 6f, 1.25f, 0.8f, -0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (_impacted) return;
        _lifetime += Time.deltaTime;

        var toPlayer = PlayerFsm.Singleton.transform.position - transform.position;
        _direction = Vector3.RotateTowards(_direction, toPlayer, 1.5f * Time.deltaTime, 1000f).normalized;
        
        var positionDelta = _direction.normalized * (Speed * Time.deltaTime);

        if (Physics.Raycast(transform.position, positionDelta, out var hit, positionDelta.magnitude,
                Fsm.GetEnvironmentalLayermask(), QueryTriggerInteraction.Ignore))
        {
            StartCoroutine(ImpactCoroutine());
            transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
        }
        
        transform.position += positionDelta;
        // if (Physics.CheckSphere(transform.position, 1f, Fsm.GetEnvironmentalLayermask(),
        //         QueryTriggerInteraction.Ignore))
        // {
        //     StartCoroutine(ImpactCoroutine());
        // }

        if (_lifetime >= 5f)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ImpactCoroutine()
    {
        _impacted = true;
        _impactParticles.Play();
        _impulse.GenerateImpulse();
        _deathCollider.SetActive(false);
        
        if (Physics.CheckSphere(transform.position, 3f, LayerMask.GetMask("Player"),
                QueryTriggerInteraction.Collide))
        {
            PlayerFsm.Singleton.InvokePlayerDeath();
        }
        
        Util.InvokeSphereEffect(transform.position - Vector3.up, Vector3.one * 8f, 1.5f, 0.8f, -1f);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
