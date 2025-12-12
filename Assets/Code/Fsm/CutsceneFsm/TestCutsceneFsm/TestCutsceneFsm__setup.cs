public partial class TestCutsceneFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CutsceneFsmState.Inactive)
            .Permit(CutsceneFsmTrigger.StartCutscene, TestCutsceneFsmState.AlignCamera);

        Machine.Configure(TestCutsceneFsmState.AlignCamera)
            .Permit(FsmTrigger.Timeout, TestCutsceneFsmState.MoveCubeForward)
            .SubstateOf(CutsceneFsmState.Active);
        
        Machine.Configure(TestCutsceneFsmState.MoveCubeForward)
            .Permit(FsmTrigger.Timeout, CutsceneFsmState.Inactive)
            .SubstateOf(CutsceneFsmState.Active);
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
    }
}