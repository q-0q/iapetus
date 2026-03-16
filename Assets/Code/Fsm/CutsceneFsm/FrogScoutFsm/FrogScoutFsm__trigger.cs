using Unity.VisualScripting;
using UnityEngine;

public partial class FrogScoutFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(FrogScoutFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        Machine.Fire(FrogScoutFsmTrigger.OnDialogueCompleted);
    }
}