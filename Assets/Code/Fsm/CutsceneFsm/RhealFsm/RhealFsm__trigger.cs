using Unity.VisualScripting;
using UnityEngine;

public partial class RhealFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(RhealFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        Machine.Fire(RhealFsmTrigger.OnDialogueCompleted);
        if (_dialogueController.currentDialogueIndex == 3)
        {
            PlayerFsm.Singleton.AcquireTrick("Tinsica");
        }
    }
}