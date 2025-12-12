public partial class TestCutsceneFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CutsceneFsmState.Inactive)
            .Permit(CutsceneFsmTrigger.StartCutscene, TestCutsceneFsmState.AlignCamera);
        
        Machine.Configure(TestCutsceneFsmState.AlignCamera)
            .Permit(FsmTrigger.Timeout, CutsceneFsmState.Inactive);
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
    }
}