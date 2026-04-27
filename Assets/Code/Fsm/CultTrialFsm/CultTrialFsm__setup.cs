using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public partial class CultTrialFsm
{


    public static event Action<CultTrialFsm> OnTrialInactive;
    
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
            .PermitIf(CultTrialFsmTrigger.OnInteracted, CultTrialFsmState.ApplyingCurse, _ => SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent) && !CultTrialManager.Singleton.isCurseEnabled, 4)
            .PermitIf(CultTrialFsmTrigger.OnInteracted, CultTrialFsmState.RemovingCurse, _ => SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent) && CultTrialManager.Singleton.isCurseEnabled, 5)
            .OnEntry(_ =>
            {
                UpdateInteractable();
                OnTrialInactive?.Invoke(this);
            })
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
            .PermitIf(FsmTrigger.Timeout, CultTrialFsmState.FirstTimeUseDialogue2, _ => !SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent))
            .PermitIf(FsmTrigger.Timeout, CultTrialFsmState.UnlockedIdle, _ => SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent), 2)
            .OnEntry(_ =>
            {
                SaveSystem.WritePlayerInGamePosition(_interactable.transform.position, "", _startingLine.rotation.eulerAngles.y);
                CultTrialManager.Singleton.EnableActiveFog();
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.CutsceneWary);
            })
            .OnExit(_ =>
            {
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Idle);
                Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
                CultTrialManager.Singleton.EnableCurse();
                StartCoroutine(CurseCameraCleanupCoroutine());
            });
        
        
        Machine.Configure(CultTrialFsmState.RemovingCurse)
            .Permit(FsmTrigger.Timeout, CultTrialFsmState.UnlockedIdle)
            .OnEntry(_ =>
            {
                Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
                SaveSystem.WritePersistentEvent(FirstTimeUsePersistentEvent);
                SaveSystem.WritePlayerInGamePosition(_interactable.transform.position, "", _startingLine.rotation.eulerAngles.y);
                CultTrialManager.Singleton.DisableActiveFog();
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.CutsceneWary);
            })
            .OnExit(_ =>
            {
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Idle);
                CultTrialManager.Singleton.DisableCurse();
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
            .Permit(CultTrialFsmTrigger.FinalKeyframeTriggered, CultTrialFsmState.Complete)
            .OnEntry(_ =>
            {
                UpdateInteractable();
                PlayerFsm.Singleton.OnCultTrialBoostTrigger();
                _interactable.SetEnabled(false);
                CultTrialManager.Singleton.StartCurseTicking(this);
                Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
            })
            .OnExit(_ =>
            {
                _interactable.SetEnabled(true);
            });

        Machine.Configure(CultTrialFsmState.Complete)
            .PermitIf(FsmTrigger.Timeout, CultTrialFsmState.FirstTimeUseDialogue4, _ => !SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent))
            .PermitIf(FsmTrigger.Timeout, CultTrialFsmState.UnlockedIdle, _ => SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent), 2)
            .OnEntry(_ =>
            {
                OnTrialInactive?.Invoke(this);
            })
            .OnExit(_ =>
            {
                Time.timeScale = 1f;
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                CultTrialManager.Singleton.isCurseTicking = false;
                PlayerFsm.Singleton.SetPositionFromSaveData();
                PlayerFsm.Singleton._timeSinceBoostStarted = 100f;
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.HardLand);
                Util.InvokeSphereEffect(PlayerFsm.Singleton.transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
            });
        
        Machine.Configure(CultTrialFsmState.FirstTimeUseDialogue4)
            .Permit(CultTrialFsmTrigger.OnDialogueCompleted, CultTrialFsmState.RemovingCurse)
            .OnEntry(_ =>
            {
                StartCoroutine(CompletionExplanationListener());
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.Duration.Add(CultTrialFsmState.ApplyingCurse, 3f);
        StateMapConfig.Duration.Add(CultTrialFsmState.RemovingCurse, 1.5f);
        StateMapConfig.Duration.Add(CultTrialFsmState.Complete, 0.5f);
        
    }
}