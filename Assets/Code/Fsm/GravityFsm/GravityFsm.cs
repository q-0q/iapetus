using UnityEngine;
using UnityEngine.Serialization;

public abstract class GravityFsm : Fsm
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

        if (GetGroundedRaycastHit(out _))
        {
            if (YVelocity < 0.5f) Machine.Fire(GravityFsmTrigger.StartFrameGrounded);
        }
        else
        {
            Machine.Fire(GravityFsmTrigger.StartFrameAerial);
        }

        if (YVelocity < 0)
        {
            Machine.Fire(GravityFsmTrigger.StartFrameWithNegativeYVelocity);
        }
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

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
                var newY = Mathf.Lerp(transform.position.y, hit.point.y, Time.deltaTime * 20);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }

    private bool GetGroundedRaycastHit(out RaycastHit hit)
    {
        var raycastLength = 0.9f * GetRaycastTimeModifier();
        Debug.DrawLine(transform.position + transform.up * raycastLength, transform.position + transform.up * raycastLength - transform.up * (raycastLength * 1.3f), Color.red);
        if (Physics.Raycast(transform.position + transform.up * raycastLength, -transform.up, out hit,
                raycastLength * 2f, ~0, QueryTriggerInteraction.Ignore))
        {
            var slope = Vector3.Angle(hit.normal, transform.up);
            return slope < 70f;
        }
        return false;
    }
}