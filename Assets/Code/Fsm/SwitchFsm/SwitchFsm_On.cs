public partial class SwitchFsm
{

    private void OnConfigure()
    {
        Machine.Configure(SwitchFsmState.On)
            .Permit(SwitchFsmTrigger.Toggle, SwitchFsmState.Off)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("On");
            });
    }
}