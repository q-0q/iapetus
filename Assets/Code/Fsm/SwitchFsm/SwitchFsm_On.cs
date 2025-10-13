public partial class SwitchFsm
{

    private void OnConfigure()
    {
        Machine.Configure(SwitchFsmState.On);
    }
}