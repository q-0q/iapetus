using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RaceFsm : Fsm
{
    public class RaceFsmState : FsmState
    {
        public static int Disabled;
        public static int Inactive;
        public static int Start;
        public static int Active;
        public static int Complete;
    }

    public class RaceFsmTrigger : FsmTrigger
    {
        public static int StartTriggered;
        public static int StartNotTriggered;
        public static int Toggle;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = RaceFsmState.Disabled;
        DisabledOnEnter();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(RaceFsmState.Inactive))
        {
            InactiveOnUpdate();
        }
        
        if (Machine.IsInState(RaceFsmState.Disabled))
        {
            DisabledOnUpdate();
        }
        
        if (Machine.IsInState(RaceFsmState.Start))
        {
            StartOnUpdate();
        }
        
        if (Machine.IsInState(RaceFsmState.Active))
        {
            ActiveOnUpdate();
        }
        
        if (Machine.IsInState(RaceFsmState.Complete))
        {
            CompleteOnUpdate();
        }
    }

    private void OnEnable()
    {
        RaceTrigger.OnTrigger += OnRaceTrigger;
        RaceTrigger.OnNotTrigger += OnNotRaceTrigger;
    }

    private void OnDisable()
    {
        RaceTrigger.OnTrigger -= OnRaceTrigger;
        RaceTrigger.OnNotTrigger -= OnNotRaceTrigger;
    }
}
