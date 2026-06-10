using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerTrickParticles : MonoBehaviour
{

    private SplineContainer _tinsicaSpline;
    private SplineContainer _tinsicaJumpSpline;
    private Transform particles;
    private bool _pending = false;
    private ParticleSystem _frontFlipParticles;

    public void InvokeTinsica()
    {
        StartCoroutine(Coroutine(_tinsicaSpline, 0.5f, _frontFlipParticles));
    }
    
    public void InvokeTinsicaJump()
    {
        StartCoroutine(Coroutine(_tinsicaJumpSpline, 0.4f, _frontFlipParticles, 0.1f));
        _frontFlipParticles.Play();
    }
    
    
    // Start is called before the first frame update
    private IEnumerator Coroutine(SplineContainer splineContainer, float d, ParticleSystem particleSystem = null, float delay = 0)
    {
        _pending = true;

        var main = particles.GetComponent<ParticleSystem>().main;
        var curve = main.startLifetime;
        curve.constantMax = d;
        curve.constantMin = d;
        main.startLifetime = curve;

        yield return null;
        _pending = false;

        yield return new WaitForSeconds(delay);
        
        if (particleSystem != null) particleSystem.Play();
        particles.GetComponent<ParticleSystem>().Play();
        var t = 0f;
        while (t < d)
        {
            if (_pending) yield break;
            transform.position = PlayerFsm.Singleton.transform.position;
            transform.rotation = PlayerFsm.Singleton.transform.rotation;
            
            var w = Util.SmoothLerp01(t / d);
            particles.position = splineContainer.EvaluatePosition(w);
            particles.rotation = Quaternion.LookRotation(particles.forward, splineContainer.EvaluateUpVector(w));
            t += Time.deltaTime;
            yield return null;
        }
        
        particles.GetComponent<ParticleSystem>().Stop();
    }
    void Start()
    {
        particles = transform.Find("Trails");
        _tinsicaSpline = transform.Find("TinsicaSpline").GetComponent<SplineContainer>();
        _tinsicaJumpSpline = transform.Find("TinsicaJumpSpline").GetComponent<SplineContainer>();
        _frontFlipParticles = transform.Find("FrontflipParticles").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
