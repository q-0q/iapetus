using System.Collections;
using System.Collections.Generic;
using Code.Fsm.TrialCollectibleFSM;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class TrialCollectibleFsm
{
    [SerializeField] private List<TrialCollectibleKeyframe> _keyframes;
    [SerializeField] private string _id;
    private int _currentKeyframeIndex;
    private float _timeOnCurrentKeyframe;
    private bool _seeking;

    private Transform _marker;
    private ParticleSystem _seekParticles;
    private ParticleSystem _activeNucleusParticles;
    private ParticleSystem _activeHaloParticles;
    
    private ParticleSystem _readyParticles;
    private Material _beaconMaterial;
    
    
    
    IEnumerator InvokeSeekParticles()
    {
        _seekParticles.Play();
        _seeking = true;
        var seekParticlesStartPosition = _currentKeyframeIndex > 0 ? _keyframes[_currentKeyframeIndex - 1].transform.position : transform
            .position;
        var seekParticlesEndPosition = _keyframes[_currentKeyframeIndex].transform.position;
        float t = 0f;
        var duration = Vector3.Distance(seekParticlesStartPosition, seekParticlesEndPosition) * 0.03f;
        // var seekMain = _seekParticles.main;
        // var curve = seekMain.startLifetime;
        // curve.constantMax = duration;
        // curve.constantMin = duration;
        // seekMain.startLifetime = curve;
        
        
        while (t < duration)
        {
            var w = t / duration;
            _seekParticles.transform.position = LerpWithArc(seekParticlesStartPosition, seekParticlesEndPosition, w, 3f);

            var haloScaleW = 0f;
            var haloScaleDuration = 0.1f;
            if (w < haloScaleDuration)
            {
                haloScaleW = Mathf.InverseLerp(haloScaleDuration, 0, w);
            } else if (w > 1f - haloScaleDuration)
            {
                _marker.position = _keyframes[_currentKeyframeIndex].transform.position;
                // _marker.rotation = _keyframes[_currentKeyframeIndex].transform.rotation;
                haloScaleW = Mathf.InverseLerp(1f - haloScaleDuration, 1f, w);
            }

            var haloScale = Mathf.Lerp(0f, 1f, haloScaleW);
            _activeHaloParticles.transform.localScale = Vector3.one * haloScale;
            _activeNucleusParticles.transform.localScale = Vector3.one * haloScale;
            
            // var nucleusShape = _activeNucleusParticles.shape;
            // nucleusShape.radius = Mathf.Lerp(0, _activeNucleusParticlesBaseRadius, t);
            // var haloShape = _activeHaloParticles.shape;
            // haloShape.radius = Mathf.Lerp(0, _activeHaloParticlesBaseRadius, t);
            
            t += Time.deltaTime;
            yield return null;
        }
        
        // _marker.gameObject.SetActive(true);
        _seekParticles.Stop();
        _seeking = false;
    }
    
    public static Vector3 LerpWithArc(Vector3 start, Vector3 end, float t, float height)
    {
        // Clamp t for safety
        t = Mathf.Clamp01(t) * 0.8f;

        // Base linear interpolation
        Vector3 position = Vector3.Lerp(start, end, t);

        // Quadratic arc: peaks at t = 0.5, zero at t = 0 and t = 1
        float arc = 4f * height * t * (1f - t);

        // Apply arc in the world-up direction
        position += Vector3.up * arc;

        return position;
    }
}