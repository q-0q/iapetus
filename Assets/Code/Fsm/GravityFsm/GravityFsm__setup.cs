public abstract partial class GravityFsm
{
    public override void SetupMachine()
    {
        base.SetupMachine();
        AerialConfigure();
        GroundedConfigure();
        RespectParentTransformConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.IsAbstract.Add(GravityFsmState.Grounded, true);
        StateMapConfig.IsAbstract.Add(GravityFsmState.Aerial, true);
        StateMapConfig.IsAbstract.Add(GravityFsmState.DontApplyYVelocity, true);
        StateMapConfig.IsAbstract.Add(GravityFsmState.RespectParentTransform, true);
    }
}