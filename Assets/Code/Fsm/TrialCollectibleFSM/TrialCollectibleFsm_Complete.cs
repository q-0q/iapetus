using UnityEngine;
using UnityEngine.SceneManagement;

public partial class TrialCollectibleFsm
{

    private void CompleteOnUpdate()
    {
        Time.timeScale = Mathf.Lerp(0.35f, 1f, Mathf.InverseLerp(0.55f, 0.7f, TimeInCurrentState()));
        
    }
    private void CompleteConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Complete)
            .Permit(FsmTrigger.Timeout, TrialCollectibleFsmState.ReadyTaken)
            .OnEntry(_ =>
            {
                OnPlayerCompletedTrial?.Invoke(this, _completionTime);
                _marker.position += Vector3.up * 3f;
                UiTimer.Singleton._active = false;
            })
            .OnExit(_ =>
            {
                Time.timeScale = 1f;
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                PlayerFsm.Singleton.SetTeleportDestination(_playerReturnTransform.position, _playerReturnTransform.forward);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.TrialTeleport);
                UiTimer.Singleton._display = false;
            });
    }
}