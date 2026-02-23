public partial class PlayerFsm
{
    private void AerialOnUpdate()
    {
        
    }

    private void AerialConfigure()
    {
        Machine.Configure(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.LockMomentum)
            .OnEntry(_ =>
            {
                _currentSlipWeight = 0;
                HandleSlipAudio();
            });
    }
}