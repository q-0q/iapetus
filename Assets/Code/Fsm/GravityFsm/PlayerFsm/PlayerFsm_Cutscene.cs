public partial class PlayerFsm
{

    private void CutsceneConfigure()
    {
        Machine.Configure(PlayerFsmState.CutsceneWary)
            .SubstateOf(PlayerFsmState.GroundMove);
    }
}