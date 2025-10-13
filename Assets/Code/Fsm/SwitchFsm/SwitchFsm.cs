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
        transform.Find("OnInteractionCollider").TryGetComponent(out OnInteractionCollider);
        transform.Find("OffInteractionCollider").TryGetComponent(out OffInteractionCollider);
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = SwitchFsmState.Off;
        OnInteractionCollider.SetEnabled(true);
        OffInteractionCollider.SetEnabled(false);
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
    }

    private void OnEnable()
    {
        OnInteractionCollider.OnInteracted += OnToggle;
        OffInteractionCollider.OnInteracted += OnToggle;
    }

    private void OnDisable()
    {
        OnInteractionCollider.OnInteracted -= OnToggle;
        OffInteractionCollider.OnInteracted -= OnToggle;
    }
}
