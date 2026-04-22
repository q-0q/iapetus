using Unity.VisualScripting;
using UnityEngine;

public partial class CultTrialFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnToggle()
    {
        Machine.Fire(CultTrialFsmTrigger.Toggle);
    }
}