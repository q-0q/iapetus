using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class CrabPassageCutsceneFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                _warningCamera.Priority = -30;
            })
            .PermitIf(CrabPassageCutsceneFsmTrigger.Trigger1, CrabPassageCutsceneFsmState.Warning1, _=> PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Interactable))
            .PermitIf(CrabPassageCutsceneFsmTrigger.Trigger2, CrabPassageCutsceneFsmState.Warning2, _=> PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Interactable))
            .PermitIf(CrabPassageCutsceneFsmTrigger.Trigger3, CrabPassageCutsceneFsmState.Channel, _=> PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Interactable));

        Machine.Configure(CrabPassageCutsceneFsmState.Warning1)
            .Permit(CrabPassageCutsceneFsmTrigger.OnDialogueCompleted, CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                // _warningCamera.Priority = 30;
                DialogueCanvas.Singleton.StartDialogue(warning1);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
                CutsceneTrigger1.gameObject.SetActive(false);
            });
        
        Machine.Configure(CrabPassageCutsceneFsmState.Warning2)
            .Permit(CrabPassageCutsceneFsmTrigger.OnDialogueCompleted, CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                // _warningCamera.Priority = 30;
                DialogueCanvas.Singleton.StartDialogue(warning2);
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
                CutsceneTrigger1.gameObject.SetActive(false);
                CutsceneTrigger2.gameObject.SetActive(false);
            });

        Machine.Configure(CrabPassageCutsceneFsmState.Channel)
            .Permit(FsmTrigger.Timeout, CutsceneFsmState.Inactive)
            .OnEntry(_ =>
            {
                OnChannel?.Invoke();
                PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.CutsceneWary);
               _channelCamera.Priority = 30;
            })
            .OnExit(_ =>
            {
                PlayerFsm.Singleton.InvokePlayerDeath();
            });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.Idle, "Eating");
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.SpeakingDefault, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.SpeakingQuestReady, "Stand");
        // StateMapConfig.AnimationTrigger.Add(CrabPassageCutsceneFsmState.QuestChannel, "Channel");
        
        StateMapConfig.CutsceneCameraDisabled.Add(CrabPassageCutsceneFsmState.Warning1, false);
        StateMapConfig.CutsceneCameraDisabled.Add(CrabPassageCutsceneFsmState.Warning2, false);
        StateMapConfig.Duration.Add(CrabPassageCutsceneFsmState.Channel, 0.75f);
    }
}

