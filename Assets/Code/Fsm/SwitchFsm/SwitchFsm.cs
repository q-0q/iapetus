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
        _interactionCollider = GetComponentInChildren<InteractionCollider>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SwitchFsmState.Off;
        _interactionCollider.SetEnabled(true);
        _powerConnector = GetComponentInChildren<PowerConnector>();
        foreach (var powerConnector in outputs)
        {
            powerConnector.AddInput(_powerConnector);
        }
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (TimeInCurrentState() > 0.5f)
        {
            _powerConnector.Source = Machine.IsInState(SwitchFsmState.On);
        }
        
    }

    private void OnEnable()
    {
        _interactionCollider.OnInteracted += OnToggle;
    }

    private void OnDisable()
    {
        _interactionCollider.OnInteracted -= OnToggle;
    }
}
