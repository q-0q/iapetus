public partial class RaceFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();
        InactiveConfigure();
        StartConfigure();
        ActiveConfigure();
        CompleteConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(RaceFsmState.Complete, 1f);
    }
}