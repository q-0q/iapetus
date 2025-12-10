public partial class CutsceneFsm
{
    private void OffConfigure()
    {
        Machine.Configure(CutsceneFsmState.Inactive)
            .Permit(CutsceneFsmTrigger.Start, CutsceneFsmState.Active);
    }
}