using System.Linq;
using UnityEngine;

public partial class PlayerFsm
{
    private void DialogueOnUpdate()
    {
        currentPotentialInteractable = null;
        
        _momentum = Mathf.Lerp(_momentum, 0f, Time.deltaTime * 7f);
        HandleCollisionMove();
        var interacted = _playerInput.actions["Interact"].WasPressedThisFrame();
        if (interacted) DialogueCanvas.Singleton.AdvanceDialogue();
        SetAnimatorMomentum();
        SetAnimatorSpeedMod();
        
        
        if (DialogueCanvas.Singleton.currentDialogueController is null) return;
        var rotationTarget = DialogueCanvas.Singleton.ControllerPosition() - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotationTarget + transform.forward * 0.01f, transform.up), Time.deltaTime * 5f);

    }

    private void DialogueConfigure()
    {
        Machine.Configure(PlayerFsmState.Dialogue)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(PlayerFsmTrigger.EndDialogue, PlayerFsmState.GroundMove);
    }
}