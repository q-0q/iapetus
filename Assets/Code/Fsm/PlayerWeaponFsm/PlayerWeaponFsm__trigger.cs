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
    
    private void OnWeaponCollision()
    {
        Machine.Fire(PlayerWeaponFsmTrigger.HitTerrain);
    }

}