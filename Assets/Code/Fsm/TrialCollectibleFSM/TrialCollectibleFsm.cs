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
        _marker.Find("ActiveParticles").transform.Find("Nucleus").TryGetComponent(out _activeNucleusParticles);
        _marker.Find("ActiveParticles").transform.Find("Halo").TryGetComponent(out _activeHaloParticles);
        _marker.Find("ReadyParticles").TryGetComponent(out _readyParticles);
        _beaconMaterial = _marker.Find("Beacon").Find("Plane").GetComponent<Renderer>().material;
        _beaconMaterial.SetFloat("_Opacity", 0);
        _readyParticles.Play();
        _seeking = false;
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
