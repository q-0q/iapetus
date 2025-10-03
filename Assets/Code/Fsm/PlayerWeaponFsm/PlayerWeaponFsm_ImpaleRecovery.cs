public partial class PlayerWeaponFsm
{
    private void ImpaleRecoveryConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.ImpaleRecovery)
            .Permit(FsmTrigger.Timeout, PlayerWeaponFsmState.Idle)
            .OnEntry(_ =>
            {
                // transform.DOShakePosition(0.5f, 0.3f);
            });
    }
}