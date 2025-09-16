using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasp;

public abstract class Fsm : MonoBehaviour
{

    public class FsmState : InheritableEnum
    {
        public static int Any;
    }

    public class FsmTrigger : InheritableEnum
    {
        public static int Timeout;
    }
    
    public Machine<int, int> Machine;
    public StateMapConfig StateMapConfig;
    
    private float _timeInCurrentState;
    protected int InitState;
    protected Animator Animator;


    private void Awake()
    {
        InheritableEnum.Initialize();
    }

    protected virtual void OnStart()
    {
        SetupMachine();
        SetupStateMaps();
        _timeInCurrentState = 0;
        
        TryGetComponent(out Animator);
    }
    
    public virtual void OnUpdate()
    {
        IncrementClockByAmount(Time.deltaTime);
    }
    
    public virtual void SetupStateMaps()
    {
        StateMapConfig = new StateMapConfig();
        StateMapConfig.Name = new StateMap<string>("No state name provided");
        StateMapConfig.Animation = new StateMap<Animation>(null);
    }

    public virtual void SetupMachine()
    {
        Machine = new Machine<int, int>(InitState);
        Machine.OnTransitioned(OnStateChanged);
    }

    public virtual void FireTriggers()
    {
        
    }

    public float TimeInCurrentState()
    {
        return _timeInCurrentState;
    }
    
    private void OnStateChanged(TriggerParams? triggerParams)
    {
        _timeInCurrentState = 0;
    }

    private void IncrementClockByAmount(float amount)
    {
        _timeInCurrentState += amount;
    }
}
