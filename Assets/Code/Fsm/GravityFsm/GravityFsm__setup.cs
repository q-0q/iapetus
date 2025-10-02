public abstract partial class GravityFsm
{
    public override void SetupMachine()
    {
        base.SetupMachine();
        Machine.Configure(GravityFsmState.Aerial)
            .OnEntryFrom(GravityFsmTrigger.StartFrameAerial, _ => { TimeInAir = 0; });
        Machine.Configure(GravityFsmState.Grounded);
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
    }
}