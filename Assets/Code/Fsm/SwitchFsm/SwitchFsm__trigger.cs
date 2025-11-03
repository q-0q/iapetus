using Unity.VisualScripting;

public partial class SwitchFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();


    }

    private void OnToggle()
    {
        Machine.Fire(SwitchFsmTrigger.Toggle);
    }
}