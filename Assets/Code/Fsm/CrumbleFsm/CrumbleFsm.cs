using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CrumbleFsm : Fsm
{
    public class CrumbleFsmState : FsmState
    {
        public static int Idle;
        public static int Breaking1;
        public static int Breaking2;
        public static int Broken;
        public static int Forming;
        public static int Breaking3;
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
        transform.Find("ReformParticles").TryGetComponent(out _reformParticleSystem);
        transform.Find("CrumbleParticles").TryGetComponent(out _crumbleParticleSystem);
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(CrumbleFsmState.Idle))
        {
            IdleOnUpdate();
        }
        if (Machine.IsInState(CrumbleFsmState.Breaking2))
        {
            Breaking2OnUpdate();
        }
        if (Machine.IsInState(CrumbleFsmState.Breaking3))
        {
            Breaking3OnUpdate();
        }
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
