using System.Linq;
using UnityEngine;

public partial class PlayerFsm
{
    private void InteractableOnUpdate()
    {
        var interacted = _playerInput.actions["Interact"].WasPressedThisFrame();
        if (currentInteractable != null && interacted) currentInteractable.TriggerInteraction();
        
        currentInteractable = _interactables
            .Where(i => i != null)
            .Where(i => Vector3.Distance(transform.position, i.transform.position) <= i.triggerRange)
            .OrderBy(i => Vector3.Distance(transform.position, i.transform.position))
            .FirstOrDefault();
        
        if(currentInteractable is null) return;
        print("boo");
        print(currentInteractable.name);
    }

    private void InteractableConfigure()
    {
        Machine.Configure(PlayerFsmState.Interactable)
            .Permit(PlayerFsmTrigger.InteractWithSwitch, PlayerFsmState.WalkToSwitchPosition)
            .Permit(PlayerFsmTrigger.StartDialogue, PlayerFsmState.WalkToDialoguePosition);
    }
}