using UnityEngine;

public partial class PlayerFsm
{
    private void SlowVaultFinishOnUpdate()
    {
        HandleTurning(VaultTurningMultiplier, true);
        MoveYOntoLedge(0f, SlowVaultFinishLedgeLerpStrength);
        transform.position += transform.forward * (SlowVaultFinishForwardSpeed * Time.deltaTime);
    }

    private void SlowVaultFinishConfigure()
    {
        Machine.Configure(PlayerFsmState.SlowVaultFinish)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            .SubstateOf(PlayerFsmState.IgnoreFailsafe)
            .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _inputBuffer.ConsumeBuffer("Jump");
                ReplaceAnimatorTrigger("SlowVaultFinish");
                YVelocity = 0;
            })
            .OnExit(_ => { 
                _momentum = 8f;
            });
    }
}