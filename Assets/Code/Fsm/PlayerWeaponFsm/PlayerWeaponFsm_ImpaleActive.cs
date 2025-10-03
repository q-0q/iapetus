using UnityEngine;

public partial class PlayerWeaponFsm
{
    private void ImpaleActiveOnUpdate()
    {
        transform.position += transform.forward * (Time.deltaTime * ImpaleActiveForwardSpeed *
                                                   (TimeInCurrentState() > ImpaleActiveForwardSpeedEndTimeThreshhold ? ImpaleActiveForwardSpeedEndModifier : 1f));
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
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.ImpaleStuck)
            .Permit(PlayerWeaponFsmTrigger.HitTerrain, PlayerWeaponFsmState.ImpaleStuck)
            .OnEntry(_ =>
            {
                _impaleActiveTargetPosition = ComputeImpaleActiveTargetPosition();
                transform.rotation =
                    Quaternion.LookRotation(_impaleActiveTargetPosition - transform.position, Vector3.up);
            });
    }
}