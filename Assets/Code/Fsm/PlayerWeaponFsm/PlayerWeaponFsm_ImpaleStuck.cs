using DG.Tweening;
using UnityEngine;

public partial class PlayerWeaponFsm
{
    private void ImpaleStuckOnUpdate()
    {
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.GrappleStartup) ||
            PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Grapple))
        {
            var forward = transform.position - new Vector3(PlayerFsm.Singleton.transform.position.x,
                transform.position.y, PlayerFsm.Singleton.transform.position.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward, Vector3.up),
                Time.deltaTime * 30f);
            PlayerWeaponTail.FinalSegmentRigidbody.AddForce((PlayerFsm.Singleton.transform.position -
                                                             PlayerWeaponTail.FinalSegmentRigidbody.transform
                                                                 .position).normalized * (250000f * Time.deltaTime));
        }
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