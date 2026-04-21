using Unity.VisualScripting;
using UnityEngine;

public partial class SavaphePitonUpperFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void OnInteracted()
    {
        ReplaceAnimatorTrigger("NotCrossedDialogue");
    }
}