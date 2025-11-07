using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CrumbleFsm : Fsm
{
    public class CrumbleFsmState : FsmState
    {
        public static int Idle;
        public static int Breaking;
        public static int Broken;
        public static int Forming;
    }

    public class CrumbleFsmTrigger : FsmTrigger
    {
        public static int PlayerSetAsParent;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = CrumbleFsmState.Idle;
        TryGetComponent(out _collider);
        TryGetComponent(out _renderer);
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerParentTransformChanged += OnPlayerParentTransformChanged;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerParentTransformChanged -= OnPlayerParentTransformChanged;
    }
}
