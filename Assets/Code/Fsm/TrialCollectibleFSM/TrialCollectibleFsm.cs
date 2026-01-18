using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TrialCollectibleFsm : Fsm
{
    public class TrialCollectibleFsmState : FsmState
    {
        public static int Disabled;
        public static int Ready;
        public static int ReadyUntaken;
        public static int ReadyTaken;
        public static int Start;
        public static int Active;
        public static int Complete;
    }

    public class TrialCollectibleFsmTrigger : FsmTrigger
    {
        public static int PlayerEnteredStartingZone;
        public static int PlayerExitedStartingZone;
        public static int PlayerEnteredEndingZone;
        public static int KeyframeTimeout;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();
        InitState = TrialCollectibleFsmState.ReadyUntaken;
        _currentKeyframeIndex = 0;
        _timeOnCurrentKeyframe = 0f;
        _marker = transform.Find("Marker");
        _marker.position = _keyframes[0].transform.position;
        transform.Find("SeekParticles").TryGetComponent(out _seekParticles);
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(TrialCollectibleFsmState.Active))
        {
            ActiveOnUpdate();
        }
    }
    
    
}
