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
        Machine.Configure(GravityFsmState.Aerial)
            .OnEntry(_ => { TimeInAir = 0;});
        Machine.Configure(GravityFsmState.Grounded);
    }

    protected float YVelocity;
    protected float GravityStrength;
    protected float TimeInAir;
    
    protected override void OnStart()
    {
        base.OnStart();
        YVelocity = 0;
        GravityStrength = 9.8f;
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
            if (YVelocity < 0) Machine.Fire(GravityFsmTrigger.StartFrameGrounded);
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
            var v3 = new Vector3(0, YVelocity * Time.deltaTime, 0);
            transform.position += v3;
            YVelocity -= (GravityStrength * GravityStrength * Time.deltaTime);
            TimeInAir += Time.deltaTime;
        }
        
        if (Machine.IsInState(GravityFsmState.Grounded))
        {
            YVelocity = 0;
        }
    }
}