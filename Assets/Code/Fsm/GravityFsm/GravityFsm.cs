using UnityEngine;
using UnityEngine.Serialization;

public  abstract partial class GravityFsm : Fsm
{
    public class GravityFsmState : FsmState
    {
        public static int Grounded;
        public static int Aerial;
        public static int DontApplyYVelocity;
        public static int RespectParentTransform;
        public static int IgnoreDepenetration;
        public static int DontApplyGustYVelocity;
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
        GustYVelocityBonus = 0;
        GravityStrength = 9.8f;
        transform.Find("DepenetrationCollider").TryGetComponent(out _depenetrationCollider);

        InstantiateSpringCollider();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Machine.IsInState(GravityFsmState.Aerial))
        {
            AerialOnUpdate();
        }
        
        if (Machine.IsInState(GravityFsmState.Grounded))
        {
            GroundedOnUpdate();
        }
        
        if (Machine.IsInState(GravityFsmState.RespectParentTransform))
        {
            RespectParentTransformOnUpdate();
        }
        else
        {
            parentTransform = null;
        }
        
        if (!Machine.IsInState(GravityFsmState.IgnoreDepenetration))
        {
            HandleDepenetration();
        }
    }


}