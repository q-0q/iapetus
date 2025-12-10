using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CutsceneFsm : Fsm
{
    public class CutsceneFsmState : FsmState
    {
        public static int Active;
        public static int Inactive;
    }

    public class CutsceneFsmTrigger : FsmTrigger
    {
        public static int Start;
        public static int End;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CutsceneFsmState.Inactive;
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
