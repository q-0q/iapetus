using UnityEngine;

public abstract class GravityFsm : Fsm
{
    public class GravityFsmState : FsmState
    {
        public static int Grounded;
        public static int Aerial;
    }

    public class GravityFsmTrigger : FsmTrigger
    {
        public static int StartFrameGrounded;
        public static int StartFrameAerial;
    }

    public override void SetupMachine()
    {
        base.SetupMachine();
        Machine.Configure(GravityFsmState.Aerial);
        Machine.Configure(GravityFsmState.Grounded);
    }

    protected float _yVelocity;
    protected float _gravityStrength;
    
    protected override void OnStart()
    {
        base.OnStart();
        _yVelocity = 0;
        _gravityStrength = 9.8f;
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
    }
    
    public override void FireTriggers()
    {
        base.FireTriggers();

        if (Physics.Raycast(transform.position, Vector3.down, 0.1f))
        {
            if (_yVelocity < 0) Machine.Fire(GravityFsmTrigger.StartFrameGrounded);
        }
        else
        {
            Machine.Fire(GravityFsmTrigger.StartFrameAerial);
        }
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Machine.IsInState(GravityFsmState.Aerial))
        {
            var v3 = new Vector3(0, _yVelocity * Time.deltaTime, 0);
            transform.position += v3;
            _yVelocity -= (_gravityStrength * _gravityStrength * Time.deltaTime);
        }
        
        if (Machine.IsInState(GravityFsmState.Grounded))
        {
            _yVelocity = 0;
        }
    }
}