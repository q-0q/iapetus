using System;
using UnityEngine;

public partial class PlayerFsm
{

    private void InventorySlowdownOnUpdate()
    {
        HandleInputMomentumChange(1f, 1.25f, true);
        HandleCollisionMove();
        SetAnimatorMomentum();
        SetAnimatorSpeedMod();
        
        if (_momentum < 1f) Machine.Jump(PlayerFsmState.Inventory);
    }

    public static event Action PlayerInventoryEntered;
    public static event Action PlayerInventoryExited;
    
    private void InventoryConfigure()
    {
        Machine.Configure(PlayerFsmState.Inventory)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(PlayerFsmTrigger.Inventory, PlayerFsmState.Idle)
            .Permit(PlayerFsmTrigger.UseIncenseBurner, PlayerFsmState.UseIncenseBurner)
            .OnEntry(_ =>
            {
                EndSurge();
                _momentum = 0;
                isSprinting = false;
                
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Inventory");
                
            })
            .OnExit(_ =>
            {
                _inputBuffer.ConsumeBuffer("Inventory");
                PlayerInventoryExited?.Invoke();
            });

        Machine.Configure(PlayerFsmState.InventorySlowdown)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Inventory)
            .OnExitFrom(GravityFsmTrigger.StartFrameAerial, _ =>
            {
                PlayerInventoryExited?.Invoke();
            })
            .OnEntry(_ =>
            {
                PlayerInventoryEntered?.Invoke();
            });
        
        Machine.Configure(PlayerFsmState.UseIncenseBurner)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Idle)
            .OnExit(_ =>
            {
                GetNearbyCultTrial(out var fsm);
                SaveSystem.WritePersistentEvent(fsm.metaName + "-unlocked");
                fsm.Unlock();
                SaveSystem.AddIncenseAmount(-1);
            });
        
    }
}