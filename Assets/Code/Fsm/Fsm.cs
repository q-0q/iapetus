using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wasp;
using Random = UnityEngine.Random;

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
    private float _previousTimeInCurrentState;
    protected int InitState;
    protected Animator Animator;


    private void Awake()
    {
        InheritableEnum.Initialize();
        OnAwake();
    }
    
    private void Start()
    {
        
        OnStart();
        SetupMachine();
        SetupStateMaps();
        _timeInCurrentState = 0;
        TryGetComponent(out Animator);
        OnStartComplete();
    }


    private void Update()
    {
        OnUpdate();
        OnFireTriggers();
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnStartComplete() { }

    
    public virtual void OnUpdate()
    {
        if (StateMapConfig.IsAbstract.GetStrict(this))
        {
            Debug.LogError("Machine somehow entered an abstract state: " + StateMapConfig.Name.Get(this));
        }
        
        IncrementClockByAmount(Time.deltaTime);
    }
    
    public virtual void SetupStateMaps()
    {
        StateMapConfig = new StateMapConfig();
        StateMapConfig.Name = new StateMap<string>("No state name provided");
        StateMapConfig.Duration = new StateMap<float>(1f);
        StateMapConfig.GravityStrengthMod = new StateMap<float>(1f);
        StateMapConfig.IsAbstract = new StateMap<bool>(false);
        StateMapConfig.AnimationTrigger = new StateMap<string>("");
        StateMapConfig.LockSpringCollider = new StateMap<bool>(false);
        StateMapConfig.TightropeLineOffset = new StateMap<Vector3>(Vector3.zero);
        StateMapConfig.TightropeLineYLerpStrength = new StateMap<float>(50f);
        StateMapConfig.CutscenePlayerDisabled = new StateMap<bool>(true);
    }

    public virtual void SetupMachine()
    {
        
        Machine = new Machine<int, int>(InitState);
        Machine.OnTransitioned(OnStateChanged);
        Machine.OnTransitionCompleted(OnStateChangedCompleted);
    }

    private void OnStateChangedCompleted(TriggerParams obj)
    {
        ReplaceAnimatorTrigger(StateMapConfig.AnimationTrigger.GetStrict(this));
    }

    public virtual void OnFireTriggers()
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

    public float PreviousTimeInCurrentState()
    {
        return _previousTimeInCurrentState;
    }
    
    protected virtual void OnStateChanged(TriggerParams? triggerParams)
    {
        _timeInCurrentState = 0;
    }
    

    private void IncrementClockByAmount(float amount)
    {
        _previousTimeInCurrentState = _timeInCurrentState;
        _timeInCurrentState += amount;
    }

    protected void ReplaceAnimatorTrigger(string trigger)
    {
        if (trigger == "") return;
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
        return output;
    }

    public static int GetEnvironmentalLayermask()
    {
        return ~LayerMask.GetMask("PlayerClothCollider", "PlayerCloth", "Player");
    }
}
