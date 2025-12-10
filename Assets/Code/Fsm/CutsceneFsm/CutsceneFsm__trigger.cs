using Unity.VisualScripting;
using UnityEngine;

public partial class CutsceneFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnToggle()
    {
        Machine.Fire(CutsceneFsmTrigger.Start);
    }
}