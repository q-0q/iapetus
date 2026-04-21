using UnityEngine;
using UnityEngine.SceneManagement;

public partial class TrialCollectibleFsm
{

    private void CompleteOnUpdate()
    {
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dying1) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dead)) return;
        Time.timeScale = Mathf.Lerp(1f, 0.15f, Mathf.InverseLerp(0, 0.2f, TimeInCurrentState()));
    }
    private void CompleteConfigure()
    {
        Machine.Configure(TrialCollectibleFsmState.Complete)
            .Permit(FsmTrigger.Timeout, TrialCollectibleFsmState.ReadyTaken)
            .OnEntry(_ =>
            {
                OnPlayerCompletedTrial?.Invoke(this, _completionTime);
                _marker.position += Vector3.up;
                UiTimer.Singleton._active = false;
            })
            .OnExit(_ =>
            {
                Time.timeScale = 1f;
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                UiTimer.Singleton._display = false;
                // if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dying1) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dead)) return;
                PlayerFsm.Singleton.SetTeleportDestination(_playerReturnTransform.position, _playerReturnTransform.forward);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.TrialTeleport);
            });
    }
}