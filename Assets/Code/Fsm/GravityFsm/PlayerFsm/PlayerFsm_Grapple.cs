using UnityEngine;

public partial class PlayerFsm
{
    private void GrappleOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        var collisionMove = ComputeCollisionMove(transform.forward * (DashForwardSpeed * Time.deltaTime));
        transform.position += collisionMove;
    }
}