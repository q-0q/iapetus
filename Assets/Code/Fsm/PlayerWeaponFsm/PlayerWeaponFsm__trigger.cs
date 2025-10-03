public partial class PlayerWeaponFsm
{
    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    public void OnPlayerImpaleEnter()
    {
        Machine.Fire(PlayerWeaponFsmTrigger.PlayerImpaleStateEntered);
    }
    
    public void OnPlayerGrappleEnter()
    {
        Machine.Fire(PlayerWeaponFsmTrigger.PlayerGrappleStateEntered);
    }
    
    private void OnWeaponCollision()
    {
        Machine.Fire(PlayerWeaponFsmTrigger.HitTerrain);
    }

}