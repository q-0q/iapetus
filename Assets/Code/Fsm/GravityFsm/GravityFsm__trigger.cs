using Code.TriggerParams;
using UnityEngine;

public abstract partial class GravityFsm
{
    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (GetGroundedRaycastHit(out var hit, true))
        {

            // if (YVelocity < 0.5f || hit.collider.gameObject.layer == LayerMask.NameToLayer("ForceSlide"))
            // {
                var param = new RaycastHitParam() { Hit = hit, kind = hit.transform.gameObject.layer == LayerMask.NameToLayer("Tightrope") ? GroundKind.Tightrope : GroundKind.Standard};
                Machine.Fire(GravityFsmTrigger.StartFrameGrounded, param);
            // }
        }
        else
        {
            Machine.Fire(GravityFsmTrigger.StartFrameAerial);
        }

        if (YVelocity < 0)
        {
            Machine.Fire(GravityFsmTrigger.StartFrameWithNegativeYVelocity);
        }

        if (_depenetrationTimer >= DepenetrationTimerDuration)
        {
            Machine.Fire(GravityFsmTrigger.DepenetrationTimeout);
        }
    }
}