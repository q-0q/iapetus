using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RaceFsm : Fsm
{
    public class RaceFsmState : FsmState
    {
        public static int Inactive;
        public static int Start;
        public static int Active;
        public static int Complete;
    }

    public class RaceFsmTrigger : FsmTrigger
    {
        public static int StartTriggered;
        public static int StartNotTriggered;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _raceStartTrigger = GetComponentInChildren<RaceStartTrigger>();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = RaceFsmState.Inactive;
        

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(RaceFsmState.Inactive))
        {
            InactiveOnUpdate();
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
        _raceStartTrigger.OnTrigger += OnStartTrigger;
        _raceStartTrigger.OnNotTrigger += OnStartNotTrigger;
    }

    private void OnDisable()
    {
        _raceStartTrigger.OnTrigger -= OnStartTrigger;
        _raceStartTrigger.OnNotTrigger -= OnStartNotTrigger;
    }
}
