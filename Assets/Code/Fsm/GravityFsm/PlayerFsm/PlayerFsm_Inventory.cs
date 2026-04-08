using System;
using UnityEngine;

public partial class PlayerFsm
{

    private void InventoryOnUpdate()
    {
        
    }

    public static event Action PlayerInventoryEntered;
    public static event Action PlayerInventoryExited;
    
    private void InventoryConfigure()
    {
        Machine.Configure(PlayerFsmState.Inventory)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(PlayerFsmTrigger.Inventory, PlayerFsmState.Idle)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Inventory");
                PlayerInventoryEntered?.Invoke();
            })
            .OnExit(_ =>
            {
                _inputBuffer.ConsumeBuffer("Inventory");
                PlayerInventoryExited?.Invoke();
            });
    }
}