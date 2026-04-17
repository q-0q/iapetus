using Unity.VisualScripting;
using UnityEngine;

public partial class CrabGuardFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(CrabGuardFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        Machine.Fire(CrabGuardFsmTrigger.OnDialogueCompleted);
    }
}