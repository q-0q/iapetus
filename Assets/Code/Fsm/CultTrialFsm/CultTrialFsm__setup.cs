using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public partial class CultTrialFsm
{


    
    private IEnumerator CurseCameraCleanupCoroutine()
    {
        var t = 0f;
        var d = 1f;
        
        var freeLook = PlayerCinemachineFreeLook.Singleton.GetFreeLook();
        var offset = freeLook.GetComponent<CinemachineCameraOffset>();

        var initialFov = freeLook.m_Lens.FieldOfView;
        var initialOffset = offset.m_Offset;

        var baseFov = PlayerCinemachineFreeLook.Singleton.GetBaseFov();
        while (t < d)
        {

            var w = Util.SmoothLerp01(t / d);
            
            freeLook.m_Lens.FieldOfView =
                Mathf.Lerp(initialFov, baseFov, w);
            
            offset.m_Offset = Vector3.Lerp(initialOffset, Vector3.zero, w);
            t += Time.deltaTime;
            yield return null;
        }

        freeLook.m_Lens.FieldOfView = baseFov;
        offset.m_Offset = Vector3.zero;
    }
    
    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CultTrialFsmState.LockedIdle)
            .Permit(CultTrialFsmTrigger.OnUnlock, CultTrialFsmState.UnlockedIdle)
            .Permit(CultTrialFsmTrigger.OnInteracted, CultTrialFsmState.LockedDialogue);

        Machine.Configure(CultTrialFsmState.LockedDialogue)
            .Permit(CultTrialFsmTrigger.OnDialogueCompleted, CultTrialFsmState.LockedIdle)
            .OnEntry(_ =>
            {
                var controller = SaveSystem.GetAllItems().Contains("IncenseBurner") ? _dialogueItem : _dialogueNoItem;
                DialogueCanvas.Singleton.StartDialogue(controller);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
            });
        
        Machine.Configure(CultTrialFsmState.FirstTimeUseDialogue1)
            .Permit(CultTrialFsmTrigger.OnDialogueCompleted, CultTrialFsmState.ApplyingCurse)
            .OnEntry(_ =>
            {
                var controller = _dialogueFirstTimeUse1;
                DialogueCanvas.Singleton.StartDialogue(controller);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
            });

        Machine.Configure(CultTrialFsmState.UnlockedIdle)
            .PermitIf(CultTrialFsmTrigger.PlayerLeftStartingLine, CultTrialFsmState.TrialActive, _ => CultTrialManager.Singleton.isCurseEnabled)
            .PermitIf(CultTrialFsmTrigger.OnInteracted, CultTrialFsmState.FirstTimeUseDialogue1, _ => !SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent))
            .PermitIf(CultTrialFsmTrigger.OnInteracted, CultTrialFsmState.FirstTimeUseDialogue2, _ => !SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent) && CultTrialManager.Singleton.isCurseEnabled, 2)
            .OnEntryFrom(CultTrialFsmTrigger.OnUnlock, _ =>
            {
                StartCoroutine(RingLightMultiplierCoroutine());
                EnableFlames();

                IEnumerator RingLightMultiplierCoroutine()
                {
                    float t = 0f;
                    float d = 0.25f;
                    while (t < d)
                    {
                        _startingLineBaseMaterial.SetFloat("_RingMultiplier", Util.SmoothLerp01(t / d));
                        t += Time.deltaTime;
                        yield return null;
                    }
                }
            });

        Machine.Configure(CultTrialFsmState.ApplyingCurse)
            .Permit(FsmTrigger.Timeout, CultTrialFsmState.FirstTimeUseDialogue2)
            .OnEntry(_ =>
            {
                SaveSystem.WritePlayerInGamePosition(_interactable.transform.position, "", _startingLine.rotation.eulerAngles.y);
                CultTrialManager.Singleton.EnableActiveFog();
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.CutsceneWary);
            })
            .OnExit(_ =>
            {
                Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
                CultTrialManager.Singleton.EnableCurse();
                StartCoroutine(CurseCameraCleanupCoroutine());
            });
        
        Machine.Configure(CultTrialFsmState.FirstTimeUseDialogue2)
            .Permit(CultTrialFsmTrigger.OnDialogueCompleted, CultTrialFsmState.UnlockedIdle)
            .OnEntry(_ =>
            {
                var controller = _dialogueFirstTimeUse2;
                DialogueCanvas.Singleton.StartDialogue(controller);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
            });
        
        Machine.Configure(CultTrialFsmState.TrialActive)
            .Permit(CultTrialFsmTrigger.PlayerTrialDeath, CultTrialFsmState.UnlockedIdle)
            .OnEntry(_ =>
            {
                _interactable.SetEnabled(false);
                CultTrialManager.Singleton.StartCurseTicking(this);
                Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
            })
            .OnExit(_ =>
            {
                _interactable.SetEnabled(true);
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.Duration.Add(CultTrialFsmState.ApplyingCurse, 3f);
        
    }
}