using UnityEngine;
using UnityEngine.Serialization;

public  abstract partial class GravityFsm : Fsm
{
    public class GravityFsmState : FsmState
    {
        public static int Grounded;
        public static int Aerial;
        public static int DontApplyYVelocity;
    }

    public class GravityFsmTrigger : FsmTrigger
    {
        public static int StartFrameGrounded;
        public static int StartFrameAerial;
        public static int StartFrameWithNegativeYVelocity;
        
    }

    public override void SetupMachine()
    {
        base.SetupMachine();
        Machine.Configure(GravityFsmState.Aerial)
            .OnEntryFrom(GravityFsmTrigger.StartFrameAerial, _ => { TimeInAir = 0;});
        Machine.Configure(GravityFsmState.Grounded);
    }

    protected float YVelocity;
    protected float GravityStrength;
    protected float TimeInAir;
    protected float MinYVelocity = -40f;
    protected float LastUpwardsY;
    
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

    public override void OnUpdate()
    {
        base.OnUpdate();

        YVelocity = Mathf.Max(YVelocity, MinYVelocity);
        if (YVelocity > 0 || Machine.IsInState(GravityFsmState.Grounded)) LastUpwardsY = transform.position.y;
            
        if (Machine.IsInState(GravityFsmState.Aerial) && !Machine.IsInState(GravityFsmState.DontApplyYVelocity))
        {
            var v3 = new Vector3(0, YVelocity * Time.deltaTime, 0);
            transform.position += v3;
            YVelocity -= (GravityStrength * GravityStrength * Time.deltaTime * StateMapConfig.GravityStrengthMod.Get(this));
            TimeInAir += Time.deltaTime;
        }
        
        if (Machine.IsInState(GravityFsmState.Grounded))
        {
            YVelocity = 0;
            if (GetGroundedRaycastHit(out var hit))
            {
                var f = 50f;
                var newY = Mathf.Lerp(transform.position.y, hit.point.y, Time.deltaTime * f);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }
}