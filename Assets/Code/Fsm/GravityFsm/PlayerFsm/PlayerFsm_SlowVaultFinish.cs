using UnityEngine;

public partial class PlayerFsm
{
    private void SlowVaultFinishOnUpdate()
    {
        HandleTurning(VaultTurningMultiplier, true);
        bool setLedgePosition = false;
        if (!UpdateLedgePosition(FaceHighLedgeHeight))
        {
            setLedgePosition = UpdateLedgePosition(0f);
        }
        else
        {
            setLedgePosition = true;
        }
        if (setLedgePosition) MoveYOntoLedge(0f, SlowVaultFinishLedgeLerpStrength);
        transform.position += transform.forward * (SlowVaultFinishForwardSpeed * Time.deltaTime);
    }

    private void SlowVaultFinishConfigure()
    {
        Machine.Configure(PlayerFsmState.SlowVaultFinish)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .SubstateOf(PlayerFsmState.TinsicaUsable)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            // .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Jump");
                YVelocity = 0;
                OnPlayerFootstep();
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                _previousWallrunSide = FlankType.None;
                _currentFlankType = FlankType.None;
                _wallsquattedSinceLeavingGround = false;
                _dashSinceLeavingGround = false;
                currentRopeSwing = null;
            })
            .OnExit(_ => { 
                _momentum = 5f;
            });
    }
}