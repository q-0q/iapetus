using UnityEngine;

public partial class PlayerFsm
{
    private void SlowVaultFinishOnUpdate()
    {
        HandleTurning(VaultTurningMultiplier, true);
        MoveYOntoLedge(0f, SlowVaultFinishLedgeLerpStrength);
        transform.position += transform.forward * (SlowVaultFinishForwardSpeed * Time.deltaTime);
    }
}