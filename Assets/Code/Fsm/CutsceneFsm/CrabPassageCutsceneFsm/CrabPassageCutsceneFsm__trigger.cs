using System;
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

    private void OnTrigger1(Collider obj)
    {
        Machine.Fire(CrabPassageCutsceneFsmTrigger.Trigger1);
    }
    
    private void OnTrigger2(Collider obj)
    {
        Machine.Fire(CrabPassageCutsceneFsmTrigger.Trigger2);
    }

    private void OnTrigger3(Collider obj)
    {
        Machine.Fire(CrabPassageCutsceneFsmTrigger.Trigger3);
    }
}