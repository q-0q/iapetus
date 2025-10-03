using UnityEngine;

public partial class PlayerWeaponFsm
{
    private void ImpaleActiveOnUpdate()
    {
        transform.position += transform.forward * (Time.deltaTime * ImpaleActivePositionLerpStrength *
                                                   (TimeInCurrentState() > 0.195f ? 0.25f : 1f));

        // transform.position = Vector3.Lerp(transform.position, _impaleActiveTargetPosition,
        //     Time.deltaTime * ImpaleActivePositionLerpStrength);
    }

    private Vector3 ComputeImpaleActiveTargetPosition()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, ImpaleActiveMaxDistance,
                LayerMask.GetMask("AimAssist"), QueryTriggerInteraction.Collide))
        {
            return hit.transform.position;
        }

        return transform.position + (transform.forward * ImpaleActiveMaxDistance);
    }

    private void ImpaleActiveConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.ImpaleActive)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleRecovery)
            .Permit(PlayerWeaponFsmTrigger.HitTerrain, PlayerWeaponFsmState.ImpaleStuck)
            .OnEntry(_ =>
            {
                _impaleActiveTargetPosition = ComputeImpaleActiveTargetPosition();
                transform.rotation =
                    Quaternion.LookRotation(_impaleActiveTargetPosition - transform.position, Vector3.up);
            });
    }
}