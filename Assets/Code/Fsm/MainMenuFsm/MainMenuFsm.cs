using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainMenuFsm : Fsm
{
    public class MainMenuFsmState : FsmState
    {
        public static int Home;
        public static int Options;
        public static int Saves;
        public static int Chapters;
    }

    public class MainMenuFsmTrigger : FsmTrigger
    {
        public static int Toggle;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        TryGetComponent(out Animator);
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = MainMenuFsmState.Home;
        
        
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
