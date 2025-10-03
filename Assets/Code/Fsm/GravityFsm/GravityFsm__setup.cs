public abstract partial class GravityFsm
{
    public override void SetupMachine()
    {
        base.SetupMachine();
        AerialConfigure();
        GroundedConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
    }
}