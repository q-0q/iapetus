public partial class CrumbleFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();
        IdleConfigure();
        BreakingConfigure();
        BrokenConfigure();
        FormingConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(CrumbleFsmState.Breaking, 2f);
        
    }
}