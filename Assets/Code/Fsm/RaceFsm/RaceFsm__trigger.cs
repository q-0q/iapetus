public partial class RaceFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();


    }

    private void OnStartTrigger()
    {
        Machine.Fire(RaceFsmTrigger.StartTriggered);
    }
    
    private void OnStartNotTrigger()
    {
        Machine.Fire(RaceFsmTrigger.StartNotTriggered);
        print("not");
    }
}