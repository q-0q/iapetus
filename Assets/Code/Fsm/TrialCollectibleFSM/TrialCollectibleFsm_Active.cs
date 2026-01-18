using UnityEngine;

public partial class TrialCollectibleFsm
{

    private void ActiveOnUpdate()
    {
        if (Physics.CheckSphere(_keyframes[_currentKeyframeIndex].transform.position, 2f, LayerMask.GetMask("Player")))
        {
            IncrementKeyframeIndex();
        }

        _timeOnCurrentKeyframe += Time.deltaTime;

        if (_currentKeyframeIndex <= _keyframes.Count - 1)
        {
            _marker.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.zero,
                _timeOnCurrentKeyframe / _keyframes[_currentKeyframeIndex].duration);
        }
    }
    
    private void ActiveConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Active)
            .Permit(TrialCollectibleFsmTrigger.PlayerEnteredEndingZone, TrialCollectibleFsmState.Complete)
            .Permit(TrialCollectibleFsmTrigger.KeyframeTimeout, TrialCollectibleFsmState.ReadyUntaken)
            .OnEntry(_ =>
            {
                IncrementKeyframeIndex();
            });
    }

    private void IncrementKeyframeIndex()
    {
        _currentKeyframeIndex++;
        if (_currentKeyframeIndex > _keyframes.Count - 1) return; 
        _marker.gameObject.SetActive(false);
        StartCoroutine(InvokeSeekParticles());
        _timeOnCurrentKeyframe = 0f;
    }
}