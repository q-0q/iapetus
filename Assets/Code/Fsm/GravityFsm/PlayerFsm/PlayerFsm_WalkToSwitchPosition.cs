using UnityEngine;

public partial class PlayerFsm
{

    private void WalkToSwitchPositionConfigure()
    {
        Machine.Configure(PlayerFsmState.WalkToSwitchPosition)
            .SubstateOf(PlayerFsmState.WalkToPosition)
            .Permit(PlayerFsmTrigger.ArriveAtWalkToPositionTarget, PlayerFsmState.InteractWithSwitch)
            .OnExit(_ =>
            {
                _currentInteractable.TriggerInteraction();
            });
    }
}