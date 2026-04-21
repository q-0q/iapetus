using Unity.VisualScripting;
using UnityEngine;

public partial class SnailMerchantFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(SnailMerchantFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        Machine.Fire(SnailMerchantFsmTrigger.OnDialogueCompleted);
    }
}