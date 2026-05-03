using Unity.VisualScripting;
using UnityEngine;

public abstract partial class CultistFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(CultistFsmTrigger.OnInteracted);
    }
    
    protected virtual void OnDialogueCompleted()
    {
        Machine.Fire(CultistFsmTrigger.OnDialogueCompleted);
    }
}