using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Code.Fsm.TrialCollectibleFSM;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class CultTrialFsm
{
    private Interactable _interactable;
    private List<CultTrialKeyframe> _keyframes;

    private DialogueController _dialogueNoItem;
    private DialogueController _dialogueItem;
    
    

    private void UpdateKeyframes()
    {
        _keyframes = transform.Find("Keyframes").GetComponentsInChildren<CultTrialKeyframe>().ToList();
        AlignStartingPosition();
    }

    private void AlignStartingPosition()
    {
        var keyframe0 = GetComponentsInChildren<CultTrialKeyframe>().ToList()[0].transform;
        var t = transform.Find("StartingLine");
        
        if (Physics.Raycast(keyframe0.position, Vector3.down, out var hit, 5f, ~LayerMask.GetMask("CultTrialStartingLine"),
                QueryTriggerInteraction.Ignore))
        {
            t.position = hit.point;
            GetComponentInChildren<Interactable>().transform.position = hit.point + Vector3.up;
            t.rotation = keyframe0.rotation;
        }
    }

    private void OnInteracted()
    {
        print("hi");
        DialogueCanvas.Singleton.StartDialogue(SaveSystem.GetAllItems().Contains("IncenseBurner") ? _dialogueItem : _dialogueNoItem);
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
    }
}