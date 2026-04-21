using UnityEngine;

public partial class PlayerFsm
{
    private void InteractWithSwitchOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        var forward = new Vector3(_walkToPositionTarget.x, transform.position.y, _walkToPositionTarget.z) - transform.position;
        var targetRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 1.5f);
    }
    
    private void InteractWithSwitchConfigure()
    {
        Machine.Configure(PlayerFsmState.InteractWithSwitch)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .OnEntry(_ =>
            {
                if (currentInteractable != null) currentInteractable.TriggerHardInteraction();
                _momentum = 0f;
            })
            .OnExit(_ =>
            {
                Animator.SetFloat("Momentum", 0f);
                Animator.SetFloat("SpeedMod", 0f);
            });
    }
}