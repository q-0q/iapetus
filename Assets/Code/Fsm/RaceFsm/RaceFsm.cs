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
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _raceStartTrigger = GetComponentInChildren<RaceStartTrigger>();
    }

    protected override void OnStart()
    {
        base.OnStart();

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        

    }

    private void OnEnable()
    {
        _raceStartTrigger.OnTrigger += OnStartTrigger;
    }

    private void OnDisable()
    {
        _raceStartTrigger.OnTrigger -= OnStartTrigger;
    }
}
