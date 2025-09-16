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

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}