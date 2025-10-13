using Code.TriggerParams;

public abstract partial class GravityFsm
{
    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (GetGroundedRaycastHit(out var hit, out var kind))
        {
            if (YVelocity < 0.5f)
            {
                var param = new RaycastHitParam() { Hit = hit };
                var trigger = kind == GroundKind.Tightrope
                    ? GravityFsmTrigger.StartFrameOnTightrope
                    : GravityFsmTrigger.StartFrameGrounded;
                print(kind);
                Machine.Fire(trigger, param);
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