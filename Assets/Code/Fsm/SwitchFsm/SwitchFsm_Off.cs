public partial class SwitchFsm
{
    private void OffConfigure()
    {
        Machine.Configure(SwitchFsmState.Off)
            .Permit(SwitchFsmTrigger.Toggle, SwitchFsmState.On)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Off");
            });
    }
}