using System.Collections.Generic;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class SwitchFsm
{
    private Interactable _interactable;
    private PowerConnector _powerConnector;

    private void StartPlayerInteraction()
    {
        InteractableParam p = new InteractableParam() { Interactable = _interactable, WalkToPositionTarget =
            _interactable.transform.position};
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.InteractWithSwitch, p);
    }
}