using System.Collections;
using Code.Misc;
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

    private void OnPlayerCultTrialDeath()
    {
        if (!SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent))
            StartCoroutine(DeathExplanationListener());
        Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
        Machine.Fire(CultTrialFsmTrigger.PlayerTrialDeath);
    }

    private IEnumerator DeathExplanationListener()
    {
        while (!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Interactable))
        {
            yield return null;
        }
        
        
        DialogueCanvas.Singleton.StartDialogue(_dialogueFirstTimeUse3);
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
        
    }
    
    private IEnumerator CompletionExplanationListener()
    {
        while (!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Interactable))
        {
            yield return null;
        }
        
        
        DialogueCanvas.Singleton.StartDialogue(_dialogueFirstTimeUse4);
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
        
    }
}