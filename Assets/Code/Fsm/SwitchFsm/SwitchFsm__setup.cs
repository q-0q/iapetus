public partial class SwitchFsm
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
        
    }
}