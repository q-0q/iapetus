using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerDashParticles : MonoBehaviour
{

    private SplineContainer _dashSpline;
    private SplineContainer _skipSpline;
    private Transform particles;
    private bool _isInvoking = false;

    public void InvokeDash()
    {
        if (_isInvoking) return;
        StartCoroutine(Coroutine(_dashSpline, 0.25f));
    }
    
    public void InvokeSkip()
    {
        if (_isInvoking) return;
        StartCoroutine(Coroutine(_skipSpline, 0.225f));
    }
    
    
    // Start is called before the first frame update
    private IEnumerator Coroutine(SplineContainer splineContainer, float d)
    {
        _isInvoking = true;

        var main = particles.GetComponent<ParticleSystem>().main;
        var curve = main.startLifetime;
        curve.constantMax = d;
        curve.constantMin = d;
        main.startLifetime = curve;
        
        
        particles.GetComponent<ParticleSystem>().Play();
        var t = 0f;
        while (t < d)
        {
            transform.position = PlayerFsm.Singleton.transform.position;
            transform.rotation = PlayerFsm.Singleton.transform.rotation;
            
            var w = Util.SmoothLerp01(t / d);
            particles.position = splineContainer.EvaluatePosition(w);
            particles.rotation = Quaternion.LookRotation(particles.forward, splineContainer.EvaluateUpVector(w));
            t += Time.deltaTime;
            yield return null;
        }
        
        particles.GetComponent<ParticleSystem>().Stop();
        _isInvoking = false;
    }
    void Start()
    {
        particles = transform.Find("Trails");
        _dashSpline = transform.Find("DashSpline").GetComponent<SplineContainer>();
        _skipSpline = transform.Find("SkipSpline").GetComponent<SplineContainer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
