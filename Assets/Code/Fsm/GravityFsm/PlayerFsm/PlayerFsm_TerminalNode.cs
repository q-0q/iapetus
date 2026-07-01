public partial class PlayerFsm
{
    private void TerminalNodeInteractConfigure()
    {
        Machine.Configure(PlayerFsmState.TerminalNodeInteract)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                EndSurge();
                _momentum = 0;
                isSprinting = false;
            });
    }
}