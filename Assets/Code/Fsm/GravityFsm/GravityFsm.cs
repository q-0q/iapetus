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

    protected override void OnStart()
    {
        base.OnStart();
        YVelocity = 0;
        GravityStrength = 9.8f;
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        UpdateYVelocityMetadata();

        if (Machine.IsInState(GravityFsmState.Aerial))
        {
            AerialOnUpdate();
        }
        
        if (Machine.IsInState(GravityFsmState.Grounded))
        {
            GroundedOnUpdate();
        }
    }

    private void UpdateYVelocityMetadata()
    {
        YVelocity = Mathf.Max(YVelocity, MinYVelocity);
        if (YVelocity > 0 || Machine.IsInState(GravityFsmState.Grounded)) LastUpwardsY = transform.position.y;
    }
}