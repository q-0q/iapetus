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
                UiTimer.Singleton._active = false;
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                PlayerFsm.Singleton.transform.position = _keyframes[0].transform.position + Vector3.forward * 5f;
                UiTimer.Singleton._display = false;
            });
    }
}