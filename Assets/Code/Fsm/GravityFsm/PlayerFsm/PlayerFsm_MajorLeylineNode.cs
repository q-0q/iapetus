public partial class PlayerFsm
{
    private void MajorLeylineNodeInteractConfigure()
    {
        Machine.Configure(PlayerFsmState.MajorLeylineNodeInteract)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                EndSurge();
                _momentum = 0;
                isSprinting = false;
            })
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Idle);
    }
}