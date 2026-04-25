using Unity.VisualScripting;
using UnityEngine;

public partial class CultTrialFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (Machine.IsInState(CultTrialFsmState.UnlockedIdle) && CultTrialManager.Singleton.isCurseEnabled)
        {
            var sqrMagnitude = Vector3.SqrMagnitude( PlayerFsm.Singleton.transform.position - _startingLine.position);
            if (sqrMagnitude >= 156f) Machine.Fire(CultTrialFsm.CultTrialFsmTrigger.PlayerLeftStartingLine);
        }
    }
    
}