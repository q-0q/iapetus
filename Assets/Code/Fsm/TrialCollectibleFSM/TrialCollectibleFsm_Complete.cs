using UnityEngine;

public partial class TrialCollectibleFsm
{

    private void CompleteConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Complete)
            .Permit(FsmTrigger.Timeout, TrialCollectibleFsmState.ReadyTaken)
            .OnEntry(_ =>
            {
                _marker.position += Vector3.up * 3f;
            });
    }
}