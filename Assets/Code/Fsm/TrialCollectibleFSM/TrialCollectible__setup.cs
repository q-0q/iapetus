public partial class TrialCollectibleFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();
        ActiveConfigure();
        CompleteConfigure();
        ReadyConfigure();
        StartConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();

        StateMapConfig.Duration.Add(TrialCollectibleFsmState.Complete, 0.3f);

    }
}