using System.Linq;
using UnityEngine;

public partial class PlayerFsm
{
    private void InteractableOnUpdate()
    {
        var interacted = _playerInput.actions["Interact"].WasPressedThisFrame();
        if (currentPotentialInteractable != null && interacted) currentPotentialInteractable.TriggerInteraction();
        
        currentPotentialInteractable = _interactables
            .Where(i => i != null)
            .Where(i => Vector3.Distance(transform.position, i.transform.position) <= i.triggerRange)
            .OrderBy(i => Vector3.Distance(transform.position, i.transform.position))
            .FirstOrDefault();

        currentInteractable = currentPotentialInteractable;
        if(currentPotentialInteractable is null) return;
    }

    private void InteractableConfigure()
    {
        Machine.Configure(PlayerFsmState.Interactable)
            .Permit(PlayerFsmTrigger.InteractWithSwitch, PlayerFsmState.WalkToSwitchPosition)
            .Permit(PlayerFsmTrigger.StartDialogue, PlayerFsmState.WalkToDialoguePosition);
    }
}