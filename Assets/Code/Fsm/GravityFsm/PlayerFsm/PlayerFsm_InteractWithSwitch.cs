using UnityEngine;

public partial class PlayerFsm
{
    private void InteractWithSwitchOnUpdate()
    {
        var forward = _currentInteractionCollider.transform.position - transform.position;
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
                _momentum = 0f;
                ReplaceAnimatorTrigger("InteractWithSwitch");
            });
    }
}