public partial class OnetimeSwitchFsm
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
        
        StateMapConfig.AnimationTrigger.Add(OnetimeSwitchFsmState.On, "On");
        StateMapConfig.AnimationTrigger.Add(OnetimeSwitchFsmState.Off, "Off");
    }
}