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

    private Transform _marker;
    private ParticleSystem _seekParticles;
    
    IEnumerator InvokeSeekParticles()
    {
        _seekParticles.Play();
        var seekParticlesStartPosition = _currentKeyframeIndex > 0 ? _keyframes[_currentKeyframeIndex - 1].transform.position : transform
            .position;
        var seekParticlesEndPosition = _keyframes[_currentKeyframeIndex].transform.position;
        float t = 0f;
        var duration = Vector3.Distance(seekParticlesStartPosition, seekParticlesEndPosition) * 0.025f;
        while (t < duration)
        {
            var w = t / duration;
            _seekParticles.transform.position = LerpWithArc(seekParticlesStartPosition, seekParticlesEndPosition, w, 3f);
            t += Time.deltaTime;
            yield return null;
        }
        _marker.position = _keyframes[_currentKeyframeIndex].transform.position;
        _marker.gameObject.SetActive(true);
        _seekParticles.Stop();
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