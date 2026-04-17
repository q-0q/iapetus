using Unity.VisualScripting;
using UnityEngine;

public partial class CrabPassageCutsceneFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(CrabPassageCutsceneFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        Machine.Fire(CrabPassageCutsceneFsmTrigger.OnDialogueCompleted);
    }
}