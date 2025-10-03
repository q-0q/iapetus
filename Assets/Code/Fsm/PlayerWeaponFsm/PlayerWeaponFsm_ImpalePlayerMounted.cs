using System;
using DG.Tweening;
using UnityEngine;

public partial class PlayerWeaponFsm
{
    private void ImpalePlayerMountedOnUpdate()
    {
        var forward = transform.position - new Vector3(PlayerFsm.Singleton.transform.position.x,
            transform.position.y, PlayerFsm.Singleton.transform.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward, Vector3.up),
            Time.deltaTime * ImpaleStuckPlayerGrappleRotationLerpStrength);
        PlayerWeaponTail.FinalSegmentRigidbody.AddForce((PlayerFsm.Singleton.transform.position -
                                                         PlayerWeaponTail.FinalSegmentRigidbody.transform.position).normalized * (ImpaleStuckPlayerGrappleTailPullForce * 10000f * Time.deltaTime));
    }

    private void ImpalePlayerMountedConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.ImpalePlayerMounted)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.Idle)
            .OnExit(_ =>
            {
                _subTransform.DOShakePosition(0.5f, 0.5f, 15);
            });
    }
}