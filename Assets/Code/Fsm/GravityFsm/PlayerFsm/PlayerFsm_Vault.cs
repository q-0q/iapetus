using UnityEngine;

public partial class PlayerFsm
{
    private void VaultOnUpdate()
    {
        _momentum = Mathf.Max(_momentum, VaultMinimumMomentum);
        var momentumWeight = ComputeMomentumWeight();
        Animator.SetFloat("SpeedMod",
            Mathf.Lerp(VaultMinimumAnimatorSpeedMod, VaultMaximumAnimatorSpeedMod, momentumWeight));
        MoveYOntoLedge(0f, VaultLedgeLerpStrength);
        SetAnimatorMomentum();
        transform.position += ComputeCollisionMove(ComputeDesiredMove());
        HandleTurning(VaultTurningMultiplier, true);
    }
}