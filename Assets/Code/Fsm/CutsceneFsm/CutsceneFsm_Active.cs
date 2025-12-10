public partial class CutsceneFsm
{

    private void OnConfigure()
    {
        Machine.Configure(CutsceneFsmState.Active)
            .Permit(CutsceneFsmTrigger.End, CutsceneFsmState.Inactive);
    }
}