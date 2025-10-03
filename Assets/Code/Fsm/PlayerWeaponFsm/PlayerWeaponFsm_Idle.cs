using UnityEngine;

public partial class PlayerWeaponFsm
{
    
    private void IdleOnUpdate()
    {
        var playerPosition = PlayerFsm.Singleton.transform.position;
        var toPlayer = playerPosition - new Vector3(transform.position.x, playerPosition.y, transform.position.z);
        var destinationPos = (playerPosition - toPlayer.normalized * IdleOrbitRadius) + Vector3.up * IdleOrbitHeight;
        transform.position =
            Vector3.Lerp(transform.position, destinationPos, Time.deltaTime * IdlePositionLerpStrength);

        var playerRotation =
            Quaternion.LookRotation((playerPosition + Vector3.up * IdleOrbitHeight) - transform.position, Vector3.up);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, playerRotation, Time.deltaTime * IdleRotationLerpStrength);
    }

    private void IdleConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.Idle)
            .Permit(PlayerWeaponFsmTrigger.PlayerImpaleStateEntered, PlayerWeaponFsmState.ImpaleStartup);
    }
}