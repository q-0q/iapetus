using Unity.VisualScripting;
using UnityEngine;

public partial class ProfessorFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        Machine.Fire(ProfessorFsmTrigger.OnInteracted);
    }
    
    private void OnDialogueCompleted()
    {
        Machine.Fire(ProfessorFsmTrigger.OnDialogueCompleted);
    }

    
}