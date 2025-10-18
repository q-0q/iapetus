using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SwitchFsm : Fsm
{
    public class SwitchFsmState : FsmState
    {
        public static int On;
        public static int Off;
    }

    public class SwitchFsmTrigger : FsmTrigger
    {
        public static int Toggle;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _interactable = GetComponentInChildren<Interactable>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SwitchFsmState.Off;
        _interactable.SetEnabled(true);
        _powerConnector = GetComponentInChildren<PowerConnector>();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (TimeInCurrentState() > 0.7f)
        {
            _powerConnector.Source = Machine.IsInState(SwitchFsmState.On);
        }
        
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnToggle;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnToggle;
    }
}
