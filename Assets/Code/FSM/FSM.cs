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

    public class Trigger : InheritableEnum
    {
        public static int Timeout;
    }
    
    public Machine<int, int> Machine;
    public StateMapConfig StateMapConfig;
    
    private float _timeInCurrentState;
    private int _initState;


    void Awake()
    {
        Machine = new Machine<int, int>(_initState);
        _timeInCurrentState = 0;
    }
    
    public void Update()
    {
        IncrementClockByAmount(Time.deltaTime);
    }
    
    public void SetupStateMaps()
    {
        StateMapConfig = new StateMapConfig();
        StateMapConfig.Name = new StateMap<string>("No state name provided");
        StateMapConfig.Animation = new StateMap<Animation>(null);
    }

    public void SetupMachine()
    {
        Machine.OnTransitioned(OnStateChanged);
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
