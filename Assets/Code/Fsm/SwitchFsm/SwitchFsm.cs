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
        InteractionCollider = GetComponentInChildren<InteractionCollider>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SwitchFsmState.Off;
        InteractionCollider.SetEnabled(true);
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
    }

    private void OnEnable()
    {
        InteractionCollider.OnInteracted += OnToggle;
    }

    private void OnDisable()
    {
        InteractionCollider.OnInteracted -= OnToggle;
    }
}
