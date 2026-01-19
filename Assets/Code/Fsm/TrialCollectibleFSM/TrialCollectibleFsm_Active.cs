using UnityEngine;

public partial class TrialCollectibleFsm
{

    private void ActiveOnUpdate()
    {
        if (Physics.CheckSphere(_keyframes[_currentKeyframeIndex].transform.position, 3f, LayerMask.GetMask("Player")))
        {
            IncrementKeyframeIndex();
        }

        _timeOnCurrentKeyframe += Time.deltaTime;

        if (_currentKeyframeIndex <= _keyframes.Count - 1)
        {
            var t = _timeOnCurrentKeyframe / _keyframes[_currentKeyframeIndex].duration;
            
        }
    }
    
    private void ActiveConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Active)
            .Permit(TrialCollectibleFsmTrigger.PlayerEnteredEndingZone, TrialCollectibleFsmState.Complete)
            .Permit(TrialCollectibleFsmTrigger.KeyframeTimeout, TrialCollectibleFsmState.ReadyUntaken)
            .OnExitFrom(TrialCollectibleFsmTrigger.KeyframeTimeout, _ =>
            {
                UiTimer.Singleton._display = false;
            })
            .OnEntry(_ =>
            {
                UiTimer.Singleton._timer = 0;
                UiTimer.Singleton._display = true;
                UiTimer.Singleton._active = true;
                IncrementKeyframeIndex();
            });
    }

    private void IncrementKeyframeIndex()
    {
        _currentKeyframeIndex++;
        if (_currentKeyframeIndex > _keyframes.Count - 1) return; 
        // _marker.gameObject.SetActive(false);
        StartCoroutine(InvokeSeekParticles());
        _timeOnCurrentKeyframe = 0f;
    }
}