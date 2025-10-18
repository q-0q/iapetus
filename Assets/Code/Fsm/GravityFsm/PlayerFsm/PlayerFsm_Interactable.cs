using System.Linq;
using UnityEngine;

public partial class PlayerFsm
{
    private void InteractableOnUpdate()
    {
        var interacted = _playerInput.actions["Interact"].WasPressedThisFrame();
        if (_currentInteractable != null && interacted) _currentInteractable.TriggerInteraction();
        
        _currentInteractable = _interactables
            .Where(i => i != null)
            .Where(i => Vector3.Distance(transform.position, i.transform.position) <= i.triggerRange)
            .OrderBy(i => Vector3.Distance(transform.position, i.transform.position))
            .FirstOrDefault();
    }

    private void InteractableConfigure()
    {
        Machine.Configure(PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.Grounded)
            .Permit(GravityFsmTrigger.StartFrameAerial, PlayerFsmState.Fall)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jumpsquat)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Skipsquat, _ => _timeSinceDashFinished <= SkipWindowDuration, 1)
            .Permit(PlayerFsmTrigger.HardTurn, PlayerFsmState.HardTurn)
            .Permit(PlayerFsmTrigger.InteractWithSwitch, PlayerFsmState.WalkToSwitchPosition)
            // .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.ImpaleGround, CanImpale)
            .PermitIf(PlayerFsmTrigger.Attack, PlayerFsmState.GrappleStartup, CanGrapple, 1)
            .OnEntry(_ =>
            {
                _wallsquattedSinceLeavingGround = false;
            });
    }
}