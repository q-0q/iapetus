using UnityEngine;

public partial class PlayerFsm
{
    private void HardLandRollOnUpdate()
    {
        transform.position += ComputeCollisionMove(transform.forward * (HardLandRollForwardSpeed * Time.deltaTime));
    }
}