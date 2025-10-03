using UnityEngine;

public partial class PlayerWeaponFsm
{
    private void ImpaleStuckRecoveryOnUpdate()
    {
        var pullback = Vector3.forward * (-ImpaleStuckRecoveryPullbackSpeed * Time.deltaTime);
        transform.position += pullback;
    }

    private void ImpaleStuckRecoveryConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.ImpaleStuckRecovery)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.Idle);
    }
}