using UnityEngine;

public partial class TrialCollectibleFsm
{
    private void ReadyConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Ready)
            .Permit(TrialCollectibleFsmTrigger.PlayerEnteredStartingZone, TrialCollectibleFsmState.Start)
            .OnEntry(_ =>
            {
                _marker.localScale = Vector3.one * 2f;
                _marker.position = transform.position;
            });

        Machine.Configure(TrialCollectibleFsmState.ReadyUntaken)
            .SubstateOf(TrialCollectibleFsmState.Ready);
        
        Machine.Configure(TrialCollectibleFsmState.ReadyTaken)
            .SubstateOf(TrialCollectibleFsmState.Ready);
    }
}