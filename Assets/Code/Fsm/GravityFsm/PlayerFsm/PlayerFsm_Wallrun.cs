using UnityEngine;

public partial class PlayerFsm
{
    private void WallrunOnUpdate()
    {
        

        if (_currentWallrunTransform != parentTransform)
        {
            parentTransform = _currentWallrunTransform;
            _previousParentTransformPosition = parentTransform.position;
            _previousParentRotation = parentTransform.rotation;
            OnParentTransformChanged(parentTransform);
        }
        
        
        SetAnimatorMomentum();
        HandleFlankAlignment();
        HandleCollisionMove(0.25f);

        transform.position +=
            ComputeCollisionMove(-_currentFlankWallNormal * (Time.deltaTime * FlankWallVacuumStrength));
    }

    private void WallrunConfigure()
    {
        Machine.Configure(PlayerFsmState.Wallrun)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .Permit(PlayerFsmTrigger.FlankOpen, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.Vault, _ => YVelocity > VaultMinimumYVelocity, 1)
            .PermitIf(PlayerFsmTrigger.FaceLedge, PlayerFsmState.MediumVaultHang, _ => true)
            .PermitIf(PlayerFsmTrigger.FaceWallStrict, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .PermitIf(PlayerFsmTrigger.FaceHighLedge, PlayerFsmState.Wallsquat,
                _ => _momentum > WallSquatMinimumMomentum)
            .OnEntry(_ =>
            {
                
                _momentum = Mathf.Max(_momentum, WallRunMinimumEntryMomentum);
                ReplaceAnimatorTrigger("Wallrun");
            })
            .OnExitFrom(PlayerFsmTrigger.Jump, _ =>
            {
                var rotationMod = _currentFlankType == FlankType.Left ? -1f : 1f;
                var forward = Quaternion.Euler(0f, WallrunJumpAngle * rotationMod, 0f) * _currentFlankWallNormal;
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            })
            .OnExit(_ => { _previousWallrunSide = _currentFlankType; });
    }
}