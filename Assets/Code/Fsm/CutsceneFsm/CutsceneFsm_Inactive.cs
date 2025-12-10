public abstract partial class CutsceneFsm
{
    private void OffConfigure()
    {
        Machine.Configure(CutsceneFsmState.Inactive)
            .Permit(CutsceneFsmTrigger.StartCutscene, CutsceneFsmState.Active);
    }
}