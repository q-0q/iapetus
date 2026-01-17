using UnityEngine;

public partial class TrialCollectibleFsm
{

    private void ActiveOnUpdate()
    {
        if (Physics.CheckSphere(_keyframes[_currentKeyframeIndex].transform.position, 2f, LayerMask.GetMask("Player")))
        {
            _currentKeyframeIndex++;
            if (_currentKeyframeIndex <= _keyframes.Count - 1) OnCurrentKeyframeUpdated();
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
                OnCurrentKeyframeUpdated();
            });
    }

    private void OnCurrentKeyframeUpdated()
    {
        _timeOnCurrentKeyframe = 0f;
        _marker.position = _keyframes[_currentKeyframeIndex].transform.position;
    }
}