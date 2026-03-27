using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;
using UnityEngine.Splines;

public class TutorialWaypointParticles : MonoBehaviour
{
    private Material _haloMaterial;
    private Transform _marker;

    public List<SplineContainer> splines;
    private int _currentSplineIndex;
    private bool isCoroutineActive;

    private const string PersistentEvent1 = "c1-tutorial-waypoint-1";
    private const string PersistentEvent2 = "c1-tutorial-waypoint-2";
    private const string PersistentEvent3 = "c1-tutorial-waypoint-3";
    
    // Start is called before the first frame update
    void Start()
    {
        _currentSplineIndex = 0;
        isCoroutineActive = false;
        _marker = transform.Find("GondolaInteractableParticles");
        _haloMaterial = _marker.Find("Halo").GetComponent<Renderer>().material;
        _marker.GetComponent<ParticleSystem>().Stop();
        
        // update spline idnex from save hgere
        if (SaveSystem.GetPersistentEventCompleted(PersistentEvent1)) _currentSplineIndex = 4;
        if (SaveSystem.GetPersistentEventCompleted(PersistentEvent2)) _currentSplineIndex = 6;
        if (SaveSystem.GetPersistentEventCompleted(PersistentEvent3)) _currentSplineIndex = splines.Count;
        else _marker.position = splines[_currentSplineIndex].EvaluatePosition(0);
    }

    // Update is called once per frame
    void Update()
    {
        _haloMaterial.SetFloat("_Weight", Mathf.Lerp(_haloMaterial.GetFloat("_Weight"), isCoroutineActive ? 0f : 1f, Time.deltaTime * 4f));
        
        if (_currentSplineIndex == splines.Count)
        {
            _haloMaterial.SetFloat("_Weight", 0f);
            return;
        };
        
        if (isCoroutineActive) return;
        
        _marker.position = Vector3.Lerp(_marker.position, splines[_currentSplineIndex].EvaluatePosition(0), Time.deltaTime * 2f);
        
        var d = Vector3.Distance(PlayerFsm.Singleton.transform.position, _marker.transform.position);
        if (d > 15f) return;
        StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        if (_currentSplineIndex == 4) SaveSystem.WritePersistentEvent(PersistentEvent1);
        if (_currentSplineIndex == 6) SaveSystem.WritePersistentEvent(PersistentEvent2);
        
        isCoroutineActive = true;
        _marker.GetComponent<ParticleSystem>().Play();
        var t = 0f;
        var d = splines[_currentSplineIndex].CalculateLength() * 0.01f;
        while (t < d)
        {
            var w = Util.SmoothLerp01(t / d);
            _marker.position = splines[_currentSplineIndex].EvaluatePosition(w);
            t += Time.deltaTime;
            yield return null;
        }
        _marker.GetComponent<ParticleSystem>().Stop();
        _currentSplineIndex++;

        if (_currentSplineIndex == splines.Count)
        {
            SaveSystem.WritePersistentEvent(PersistentEvent3);
        }
        
        isCoroutineActive = false;
    }
}
