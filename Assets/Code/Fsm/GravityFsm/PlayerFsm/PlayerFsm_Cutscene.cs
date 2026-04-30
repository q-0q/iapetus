public partial class PlayerFsm
{

    private void CutsceneConfigure()
    {
        Machine.Configure(PlayerFsmState.CutsceneWary)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
            });
        
        Machine.Configure(PlayerFsmState.CutsceneIdle)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
            });
    }
}