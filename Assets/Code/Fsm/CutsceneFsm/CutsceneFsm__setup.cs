public partial class CutsceneFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();
        OnConfigure();
        OffConfigure();
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.AnimationTrigger.Add(CutsceneFsmState.Active, "Active");
        StateMapConfig.AnimationTrigger.Add(CutsceneFsmState.Inactive, "Inactive");
    }
}