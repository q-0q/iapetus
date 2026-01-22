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

        if (_seeking)
        {
            _beaconMaterial.SetFloat("_Opacity", 0);
        }
        else if (_currentKeyframeIndex <= _keyframes.Count - 1)
        {
            var newOpacity = Mathf.InverseLerp(25f, 35f,
                Vector3.Distance(_keyframes[_currentKeyframeIndex].transform.position,
                    PlayerFsm.Singleton.transform.position));
            _beaconMaterial.SetFloat("_Opacity", newOpacity);
            
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
            .OnExit(_ =>
            {
                _activeParticles.Stop();
                _activeParticles.Clear();
                _activeFinalParticles.Stop();
                _activeFinalParticles.Clear();
            })
            .OnEntry(_ =>
            {
                UiTimer.Singleton._timer = 0;
                UiTimer.Singleton._display = true;
                UiTimer.Singleton._active = true;
                _keyframes[0].EnableCameraZone();
                _initialCameraBehaviorZone.gameObject.SetActive(false);
                _activeParticles.Play();
                _activeFinalParticles.Play();
                _activeFinalParticles.transform.localScale = Vector3.zero;
                IncrementKeyframeIndex();
                OnPlayerBeganTrial?.Invoke();
            });
    }

    private void IncrementKeyframeIndex()
    {
        _keyframes[_currentKeyframeIndex].DisableCameraZone();
        _keyframeTriggerParticles.Play();
        _currentKeyframeIndex++;
        if (_currentKeyframeIndex > _keyframes.Count - 1) return; 
        _keyframes[_currentKeyframeIndex].EnableCameraZone();
        // _marker.gameObject.SetActive(false);
        StartCoroutine(InvokeSeekParticles());
        _timeOnCurrentKeyframe = 0f;
    }
}