using UnityEngine;

public partial class TrialCollectibleFsm
{

    private void StartConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Start)
            .Permit(TrialCollectibleFsmTrigger.PlayerExitedStartingZone, TrialCollectibleFsmState.Active)
            .OnEntry(_ =>
            {
                _currentKeyframeIndex = 0;
                _timeOnCurrentKeyframe = 0;
                _marker.position = transform.position + Vector3.up * 3f;
            });
    }
}