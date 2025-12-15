public abstract partial class CutsceneFsm
{
    private void OffConfigure()
    {
        Machine.Configure(CutsceneFsmState.Inactive)
            .OnEntry(_ => { CutsceneManager.Singleton.ClearActiveCutscene(); })
            .OnExit(_ => { CutsceneManager.Singleton.SetActiveCutscene(this); });
    }
}