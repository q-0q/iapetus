using UnityEngine;

public partial class PlayerFsm
{
    private void InteractWithSwitchOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        var forward = new Vector3(_currentInteractionCollider.transform.position.x, transform.position.y, _currentInteractionCollider.transform.position.z) - transform.position;
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