public abstract partial class CutsceneFsm
{
    private void OffConfigure()
    {
        Machine.Configure(CutsceneFsmState.Inactive)
            .OnExit(_ => { CutsceneManager.Singleton.SetActiveCutscene(this); });
    }
}