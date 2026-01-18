using UnityEngine;

public partial class TrialCollectibleFsm
{
    private void ReadyConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Ready)
            .Permit(TrialCollectibleFsmTrigger.PlayerEnteredStartingZone, TrialCollectibleFsmState.Active)
            .OnEntry(_ =>
            {
                _marker.localScale = Vector3.one * 2f;
                _marker.position = _keyframes[0].transform.position;
                _marker.gameObject.SetActive(true);
                print("set");
            });

        Machine.Configure(TrialCollectibleFsmState.ReadyUntaken)
            .SubstateOf(TrialCollectibleFsmState.Ready);
        
        Machine.Configure(TrialCollectibleFsmState.ReadyTaken)
            .SubstateOf(TrialCollectibleFsmState.Ready);
    }
}