public abstract partial class GravityFsm
{
    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (GetGroundedRaycastHit(out _))
        {
            if (YVelocity < 0.5f) Machine.Fire(GravityFsmTrigger.StartFrameGrounded);
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