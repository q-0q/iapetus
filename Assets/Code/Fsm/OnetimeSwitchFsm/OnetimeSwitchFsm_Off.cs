public partial class OnetimeSwitchFsm
{
    private void OffConfigure()
    {
        Machine.Configure(OnetimeSwitchFsmState.Off)
            .Permit(OnetimeSwitchFsmTrigger.Toggle, OnetimeSwitchFsmState.On);
    }
}