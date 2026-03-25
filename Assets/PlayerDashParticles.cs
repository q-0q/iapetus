using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerDashParticles : MonoBehaviour
{

    private SplineContainer _spline;
    private Transform particles;
    private bool _isInvoking = false;

    public void Invoke()
    {
        if (_isInvoking) return;
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            _isInvoking = true;
            particles.GetComponent<ParticleSystem>().Play();
            var t = 0f;
            var d = 0.2f;
            while (t < d)
            {
                transform.position = PlayerFsm.Singleton.transform.position;
                transform.rotation = PlayerFsm.Singleton.transform.rotation;
                
                var w = Util.SmoothLerp01(t / d);
                particles.position = _spline.EvaluatePosition(w);
                particles.rotation = Quaternion.LookRotation(particles.forward, _spline.EvaluateUpVector(w));
                t += Time.deltaTime;
                yield return null;
            }
            
            particles.GetComponent<ParticleSystem>().Stop();
            _isInvoking = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        particles = transform.Find("Trails");
        _spline = GetComponentInChildren<SplineContainer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
