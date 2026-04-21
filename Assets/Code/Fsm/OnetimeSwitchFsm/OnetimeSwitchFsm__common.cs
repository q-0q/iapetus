using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class OnetimeSwitchFsm
{
    private Interactable _interactable;
    private PowerConnector _powerConnector;
    
    public static Action<OnetimeSwitchFsm> OnOnetimeSwitchFsmTurnedOn;
    public string persistentEvent;
    private const string _eventPath = "event:/OnetimeSwitch";

    private void StartPlayerInteraction()
    {
        InteractableParam p = new InteractableParam() { Interactable = _interactable, WalkToPositionTarget =
            _interactable.transform.position};
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.InteractWithSwitch, p);
    }
    
}