using Unity.VisualScripting;
using UnityEngine;

public partial class OnetimeSwitchFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnToggle()
    {
        Machine.Fire(OnetimeSwitchFsmTrigger.Toggle);
    }
}