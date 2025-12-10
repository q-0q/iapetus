using Unity.VisualScripting;
using UnityEngine;

public abstract partial class CutsceneFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnToggle()
    {
        Machine.Fire(CutsceneFsmTrigger.StartCutscene);
    }
}