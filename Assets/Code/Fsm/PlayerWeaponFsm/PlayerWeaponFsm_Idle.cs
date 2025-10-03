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
        // var inputMovementVector3 = PlayerFsm.Singleton.GetInputMovementVector3();
        // if (inputMovementVector3.magnitude < PlayerFsm.InputMagnitudeThreshhold)
        //     inputMovementVector3 = transform.forward;
        // var inputRotation = Quaternion.LookRotation(inputMovementVector3.normalized, Vector3.up);
        // var destinationRotation = Quaternion.Slerp(playerRotation, inputRotation, 0.f);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, playerRotation, Time.deltaTime * IdleRotationLerpStrength);
    }

    private void IdleConfigure()
    {
        Machine.Configure(PlayerWeaponFsmState.Idle)
            .Permit(PlayerWeaponFsmTrigger.PlayerImpaleStateEntered, PlayerWeaponFsmState.ImpaleStartup);
    }
}