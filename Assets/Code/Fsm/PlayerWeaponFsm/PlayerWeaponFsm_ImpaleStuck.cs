using DG.Tweening;
using UnityEngine;

public partial class PlayerWeaponFsm
{
    private void ImpaleStuckOnUpdate()
    {

    }

    private void ImpaleStuckConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.ImpaleStuck)
            .PermitIf(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleStuckRecovery,
                _ => !PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Grapple))
            .OnEntry(_ =>
            {
                // _impulseSource.GenerateImpulse();
                HitstopManager.Singleton.StartHitstop(0.075f);
                transform.DOShakePosition(0.5f, 0.3f);
            });
    }
}