using System.Collections;
using System.Collections.Generic;
using System.Linq; // Required for .ToList()
using Cinemachine;
using Code.Misc;
using UnityEngine;
using UnityEngine.Splines;

public class WaterCleanseStation : MonoBehaviour
{
    private Interactable _interactable;
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineTrackedDolly _dolly;
    private Transform _marker;
    private SplineContainer _spline;

    public float cameraCoroutineDurationMod = 1f;

    // Optimized: Store the Component directly to avoid GetComponent calls later
    private Dictionary<DualWaterPoint, bool> _pointsActivated;
    private const float PointActivationDistance = 25f;
    // Pre-calculate the square of the distance for performance
    private float _sqrActivationDistance;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _dolly = _virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        _marker = transform.Find("Marker");
        _spline = GetComponentInChildren<SplineContainer>();
        
        _pointsActivated = new Dictionary<DualWaterPoint, bool>();
        _sqrActivationDistance = PointActivationDistance * PointActivationDistance;

        foreach (var dwp in GetComponentsInChildren<DualWaterPoint>())
        {
            _pointsActivated[dwp] = false;
        }
    }

    private void OnEnable() => _interactable.OnInteracted += OnInteracted;
    private void OnDisable() => _interactable.OnInteracted -= OnInteracted;

    private void OnInteracted() => StartCoroutine(MainCoroutine());

    private IEnumerator MainCoroutine()
    {
        _virtualCamera.Priority = 30;
        _marker.GetComponentInChildren<ParticleSystem>().Play();
        CutsceneManager.Singleton.SetPseudoCutsceneActive();

        float t = 0f;
        float duration = 3f;

        while (t < Mathf.Max(duration, duration / cameraCoroutineDurationMod))
        {
            float markerW = Util.SmoothLerp01(t / duration);
            _marker.position = _spline.EvaluatePosition(markerW);
            
            float cameraW = Util.SmoothLerp01(t * cameraCoroutineDurationMod / duration);
            _dolly.m_PathPosition = cameraW;
            
            InvokePointCoroutines();

            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        _virtualCamera.Priority = -20;
        CutsceneManager.Singleton.ClearPseudoCutsceneActive();
    }

    private void InvokePointCoroutines()
    {
        var points = _pointsActivated.Keys.ToList();

        foreach (var dwp in points)
        {
            // Skip if already on
            if (_pointsActivated[dwp]) continue;

            // PERFORMANCE: Use sqrMagnitude to avoid expensive Square Root calculations
            float sqrDist = (_marker.position - dwp.transform.position).sqrMagnitude;
                
            if (sqrDist < _sqrActivationDistance)
            {
                StartCoroutine(PointCoroutine(dwp));
            }
        }
    }

    private IEnumerator PointCoroutine(DualWaterPoint dwp)
    {
        _pointsActivated[dwp] = true;
        if (dwp.InvokeHalo) StartCoroutine(InvokeSphereEffect(dwp.transform.position, dwp.DesiredRadius * 1.5f));

        float t = 0f;
        float duration = 0.5f;

        while (t < duration)
        {
            float w = Util.SmoothLerp01(t / duration);
            dwp.Radius = Mathf.Lerp(0, dwp.DesiredRadius, w);
            t += Time.deltaTime;
            yield return null;
        }
        
        dwp.Radius = dwp.DesiredRadius; // Ensure it snaps to final value
    }
    
    private IEnumerator InvokeSphereEffect(Vector3 position, float finalScale)
    {

        yield return new WaitForSeconds(0.25f);
        
        var haloPrefab = Resources.Load("Prefab/WaterCleanseHalo") as GameObject;
        var haloObject = Object.Instantiate(haloPrefab, position,
            Quaternion.identity, null);
        var material = haloObject.GetComponent<Renderer>().material;
        
        float t = 0f;
        float duration = 0.15f;

        while (t < duration)
        {
            float w = Util.SmoothLerp01(t / duration);
            haloObject.transform.localScale = Vector3.one * Mathf.Lerp(finalScale * 0.5f, finalScale, w);
            material.SetFloat("_Weight", 1f);
            t += Time.deltaTime;
            yield return null;
        }
        
        t = 0f;
        duration = 1f;

        while (t < duration)
        {
            float w = Util.SmoothLerp01(t / duration);
            haloObject.transform.localScale = Vector3.one * Mathf.Lerp(finalScale, finalScale + 5f, w);
            material.SetFloat("_Weight", 1f - w);
                
            t += Time.deltaTime;
            yield return null;
        }
        
        Destroy(haloObject);
    }
}