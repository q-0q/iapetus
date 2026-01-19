using UnityEngine;

public partial class TrialCollectibleFsm
{

    private void CompleteOnUpdate()
    {
        Time.timeScale = Mathf.Lerp(0.35f, 1f, Mathf.InverseLerp(0.2f, 0.4f, TimeInCurrentState()));
    }
    private void CompleteConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Complete)
            .Permit(FsmTrigger.Timeout, TrialCollectibleFsmState.ReadyTaken)
            .OnEntry(_ =>
            {
                _marker.position += Vector3.up * 3f;
                UiTimer.Singleton._active = false;
            })
            .OnExit(_ =>
            {
                Time.timeScale = 1f;
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                PlayerFsm.Singleton.transform.position = _keyframes[0].transform.position + Vector3.forward * 5f;
                UiTimer.Singleton._display = false;
            });
    }
}