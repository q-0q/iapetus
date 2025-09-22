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
        StateMapConfig.Duration = new StateMap<float>(1f);
        StateMapConfig.GravityStrengthMod = new StateMap<float>(1f);
    }

    public virtual void SetupMachine()
    {
        Machine = new Machine<int, int>(InitState);
        Machine.OnTransitioned(OnStateChanged);
    }

    public virtual void FireTriggers()
    {
        if (TimeInCurrentState() >= StateMapConfig.Duration.Get(this))
        {
            Machine.Fire(FsmTrigger.Timeout);
        }
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

    protected void ReplaceAnimatorTrigger(string trigger)
    {
        foreach (var t in Animator.parameters)
        {
            if (t.type != AnimatorControllerParameterType.Trigger) continue;
            if (t.name == trigger) Animator.SetTrigger(t.name);
            else Animator.ResetTrigger(t.name);
        }
    }

    protected float GetRaycastTimeModifier()
    {
        float baseFps = 300f; // base fps my machine typically gets during dev
        var currentFPS = (1.0f / Time.deltaTime);
        float output = Mathf.Lerp(1f, 1.5f, Mathf.InverseLerp(baseFps, 0, currentFPS));
        print(output);
        return output;
    }
}
