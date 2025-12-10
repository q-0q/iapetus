public abstract partial class CutsceneFsm
{

    private void OnConfigure()
    {
        Machine.Configure(CutsceneFsmState.Active)
            .Permit(CutsceneFsmTrigger.EndCutscene, CutsceneFsmState.Inactive);
    }
}