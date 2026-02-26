using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class OnetimeSwitchFsm : Fsm
{
    public class OnetimeSwitchFsmState : FsmState
    {
        public static int On;
        public static int Off;
    }

    public class OnetimeSwitchFsmTrigger : FsmTrigger
    {
        public static int Toggle;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
        TryGetComponent(out Animator);
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = OnetimeSwitchFsmState.Off;
        _interactable.SetEnabled(true);
        _powerConnector = GetComponentInChildren<PowerConnector>();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (TimeInCurrentState() > 0.7f)
        {
            _powerConnector.Source = Machine.IsInState(OnetimeSwitchFsmState.On);
        }
        
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += StartPlayerInteraction;
        _interactable.OnHardInteracted += OnToggle;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= StartPlayerInteraction;
        _interactable.OnHardInteracted -= OnToggle;
    }
}
