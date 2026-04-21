using System.Linq;
using UnityEngine;

public partial class PlayerFsm
{
    private void DialogueOnUpdate()
    {
        currentPotentialInteractable = null;

        var newMomentum = Mathf.Lerp(_momentum, 0f, Time.deltaTime * 7f);
        _momentum = newMomentum;
        if (_momentum < 2f) ReplaceAnimatorTrigger("Idle");
        HandleCollisionMove();
        var interacted = _playerInput.actions["Interact"].WasPressedThisFrame();
        if (interacted) DialogueCanvas.Singleton.AdvanceDialogue();
        SetAnimatorMomentum();
        SetAnimatorSpeedMod();
        Animator.SetLayerWeight(1, 0);
        
        
        if (DialogueCanvas.Singleton.currentDialogueController is null) return;
        
        var lookAt = DialogueCanvas.Singleton.currentDialogueController.LookAtOverride == null
            ? DialogueCanvas.Singleton.currentDialogueController.transform
            : DialogueCanvas.Singleton.currentDialogueController.LookAtOverride;
        
        var rotationTarget = lookAt.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotationTarget + transform.forward * 0.01f, transform.up), Time.deltaTime * 5f);

    }

    private void DialogueConfigure()
    {
        Machine.Configure(PlayerFsmState.Dialogue)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(PlayerFsmTrigger.EndDialogue, PlayerFsmState.Idle);
    }
}