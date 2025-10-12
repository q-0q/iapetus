using UnityEngine;

public partial class PlayerFsm
{
    private void TightropeGroundMoveOnUpdate()
    {

    }

    private void TightropeGroundMoveConfigure()
    {
        Machine.Configure(PlayerFsmState.TightropeGroundMove)
            .SubstateOf(PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
                ReplaceAnimatorTrigger("GroundMove");
            });
    }
}