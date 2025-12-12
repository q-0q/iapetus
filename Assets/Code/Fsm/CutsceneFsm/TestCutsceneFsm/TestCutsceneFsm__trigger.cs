using Unity.VisualScripting;
using UnityEngine;

public partial class TestCutsceneFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(CutsceneFsmTrigger.StartCutscene);
    }
}