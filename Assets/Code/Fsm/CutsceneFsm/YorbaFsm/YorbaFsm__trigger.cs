using Unity.VisualScripting;
using UnityEngine;

public partial class YorbaFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(YorbaFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        if (_dialogueController.currentDialogueIndex == 1) SaveSystem.WritePersistentEvent(ExpositionPersistentEvent);
        Machine.Fire(YorbaFsmTrigger.OnDialogueCompleted);
    }
}