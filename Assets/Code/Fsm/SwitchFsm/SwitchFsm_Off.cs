public partial class SwitchFsm
{
    private void OffConfigure()
    {
        Machine.Configure(SwitchFsmState.Off)
            .OnEntry(_ =>
            {
                ReplaceAnimatorTrigger("Off");
            });
    }
}