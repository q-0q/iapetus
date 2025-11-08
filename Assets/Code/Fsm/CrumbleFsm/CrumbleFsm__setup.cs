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
        
        
    }
}