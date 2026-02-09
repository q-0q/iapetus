using Unity.VisualScripting;
using UnityEngine;

public partial class SavapheFsm
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