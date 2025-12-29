using UnityEngine;

public partial class PlayerFsm
{
    private void SlowVaultFinishOnUpdate()
    {
        HandleTurning(VaultTurningMultiplier, true);
        if (!UpdateLedgePosition(FaceHighLedgeHeight))
        {
            UpdateLedgePosition(0f);
        }
        MoveYOntoLedge(0f, SlowVaultFinishLedgeLerpStrength);
        transform.position += transform.forward * (SlowVaultFinishForwardSpeed * Time.deltaTime);
    }

    private void SlowVaultFinishConfigure()
    {
        Machine.Configure(PlayerFsmState.SlowVaultFinish)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            // .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Jump");
                YVelocity = 0;
                
            })
            .OnExit(_ => { 
                _momentum = 5f;
            });
    }
}