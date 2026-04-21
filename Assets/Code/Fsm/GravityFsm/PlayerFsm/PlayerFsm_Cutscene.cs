public partial class PlayerFsm
{

    private void CutsceneConfigure()
    {
        Machine.Configure(PlayerFsmState.CutsceneWary)
            .SubstateOf(GravityFsmState.RespectParentTransform);
        
        Machine.Configure(PlayerFsmState.CutsceneIdle)
            .SubstateOf(GravityFsmState.RespectParentTransform);
    }
}