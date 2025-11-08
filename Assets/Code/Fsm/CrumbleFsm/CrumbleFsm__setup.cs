public partial class CrumbleFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();
        IdleConfigure();
        Breaking1Configure();
        Breaking2Configure();
        Breaking3Configure();
        BrokenConfigure();
        FormingConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(CrumbleFsmState.Breaking1, 0.75f);
        StateMapConfig.Duration.Add(CrumbleFsmState.Breaking2, 0.5f);
        StateMapConfig.Duration.Add(CrumbleFsmState.Breaking3, 0.75f);
        
    }
}