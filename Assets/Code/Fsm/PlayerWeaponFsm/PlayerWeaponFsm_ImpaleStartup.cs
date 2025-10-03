using DG.Tweening;
using UnityEngine;

public partial class PlayerWeaponFsm
{
    private void ImpaleStartupOnUpdate()
    {
        var forward = PlayerFsm.Singleton.GetInputMovementVector3().normalized;
        var orbitCenter = PlayerFsm.Singleton.transform.position + forward * ImpaleStartupOrbitCenterForwardOffset;
        var pullback = Vector3.forward * (-ImpaleStartupPullbackSpeed * Time.deltaTime);
        _subTransform.localPosition += pullback;

        var toOrbitCenter = orbitCenter - new Vector3(transform.position.x, orbitCenter.y, transform.position.z);
        var destinationPos = (orbitCenter - toOrbitCenter.normalized * ImpaleStartupOrbitRadius) +
                             (Vector3.up * ImpaleStartupOrbitHeight);
        transform.position = Vector3.Lerp(transform.position, destinationPos,
            Time.deltaTime * ImpaleStartupPositionLerpStrength);

        if (forward.magnitude < PlayerFsm.InputMagnitudeThreshhold) forward = transform.forward;
        var destinationRot = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, destinationRot,
            Time.deltaTime * ImpaleStartupRotationLerpStrength);
    }

    private void ImpaleStartupConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.ImpaleStartup)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleActive)
            .OnEntry(_ =>
            {
                _subTransform.DOShakePosition(0.3f, 0.3f);
                transform.rotation = Quaternion.LookRotation(PlayerFsm.Singleton.transform.forward, Vector3.up);
            });
    }
}