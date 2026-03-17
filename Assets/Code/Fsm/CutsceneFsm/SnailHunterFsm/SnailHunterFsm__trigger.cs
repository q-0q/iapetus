using Unity.VisualScripting;
using UnityEngine;

public partial class SnailHunterFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(SnailHunterFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        Machine.Fire(SnailHunterFsmTrigger.OnDialogueCompleted);
    }
}