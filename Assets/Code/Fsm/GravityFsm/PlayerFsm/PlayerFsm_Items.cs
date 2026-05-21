using System;
using UnityEngine;

public partial class PlayerFsm
{

    private void ItemSlowdownOnUpdate()
    {
        HandleInputMomentumChange(1f, 1.25f, true);
        HandleCollisionMove();
        SetAnimatorMomentum();
        SetAnimatorSpeedMod();
        
        if (_momentum < 1f) Machine.Fire(FsmTrigger.Timeout); // Insane black magic wizardry... in the world of the digital, we are our own gods.
    }

    public static event Action PlayerInventoryEntered;
    public static event Action PlayerInventoryExited;
    public static event Action PlayerMapEntered;
    public static event Action PlayerMapExited;
    
    private void ItemsConfigure()
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

        Machine.Configure(PlayerFsmState.ItemSlowdown)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall);

        Machine.Configure(PlayerFsmState.InventorySlowdown)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Inventory)
            .SubstateOf(PlayerFsmState.ItemSlowdown)
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
                if (SaveSystem.GetIncenseAmount() == 0)
                {
                    SaveSystem.AdvanceCultLocationCampId();
                }
            });
        
        Machine.Configure(PlayerFsmState.UseMap)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(PlayerFsmTrigger.Inventory, PlayerFsmState.Idle)
            .Permit(PlayerFsmTrigger.Map, PlayerFsmState.Idle)
            .OnEntry(_ =>
            {
                EndSurge();
                _momentum = 0;
                isSprinting = false;
                
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Map");
            })
            .OnExit(_ =>
            {
                _inputBuffer.ConsumeBuffer("Map");
                PlayerMapExited?.Invoke();
            });
        
        
        Machine.Configure(PlayerFsmState.MapSlowdown)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.UseMap)
            .SubstateOf(PlayerFsmState.ItemSlowdown)
            .OnExitFrom(GravityFsmTrigger.StartFrameAerial, _ =>
            {
                PlayerMapExited?.Invoke();
            })
            .OnEntry(_ =>
            {
                PlayerMapEntered?.Invoke();
            });
        
    }
}