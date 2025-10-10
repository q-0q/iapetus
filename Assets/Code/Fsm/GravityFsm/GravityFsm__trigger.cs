using Code.TriggerParams;

public abstract partial class GravityFsm
{
    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (GetGroundedRaycastHit(out var hit))
        {
            if (YVelocity < 0.5f)
            {
                var param = new RaycastHitParam() { Hit = hit };
                Machine.Fire(GravityFsmTrigger.StartFrameGrounded, param);
            }
        }
        else
        {
            Machine.Fire(GravityFsmTrigger.StartFrameAerial);
        }

        if (YVelocity < 0)
        {
            Machine.Fire(GravityFsmTrigger.StartFrameWithNegativeYVelocity);
        }
    }
}