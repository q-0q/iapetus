using UnityEngine;

public partial class PlayerFsm
{

    private void WallsquatOnUpdate()
    {
        YVelocity = Mathf.Lerp(0, -10f, Mathf.InverseLerp(WallsquatMinimumDuration + 0.1f, WallsquatMinimumDuration + 0.3f, TimeInCurrentState()));
    }
    
    private void WallsquatConfigure()
    {
        Machine.Configure(PlayerFsmState.Wallsquat)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.ForceWallRotation)
            // .SubstateOf(GravityFsmState.DontApplyYVelocity)
            .SubstateOf(GravityFsmState.RespectParentTransform)
            .Permit(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.Landsquat)
            .PermitIf(PlayerFsmTrigger.FaceOpen, PlayerFsmState.Fall, _ => TimeInCurrentState() > WallsquatMinimumDuration)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Wallstep,
                _ => TimeInCurrentState() > WallstepMinimumDuration, 1)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                LastUpwardsY = transform.position.y;
                _wallsquattedSinceLeavingGround = true;

                landEventInstance.start();
            })
            .OnExit(_ =>
            {
                YVelocity = 0;
            })
            .OnExitFrom(PlayerFsmTrigger.FaceOpen, _ => { _momentum = 0; });
    }
}