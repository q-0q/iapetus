using UnityEngine;

public partial class PlayerFsm
{
    private void WallrunOnUpdate()
    {
        SetAnimatorMomentum();
        HandleFlankAlignment();
        HandleCollisionMove();

        transform.position +=
            ComputeCollisionMove(-_currentFlankWallNormal * (Time.deltaTime * FlankWallVacuumStrength));
    }
}