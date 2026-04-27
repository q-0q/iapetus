using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Code.Fsm.TrialCollectibleFSM;
using Code.Misc;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public partial class CultTrialFsm
{
    public string metaName = "";
    
    private Interactable _interactable;
    private List<CultTrialKeyframe> _keyframes;

    private DialogueController _dialogueNoItem;
    private DialogueController _dialogueItem;
    private DialogueController _dialogueFirstTimeUse1;
    private DialogueController _dialogueFirstTimeUse2;
    private DialogueController _dialogueFirstTimeUse3;
    private DialogueController _dialogueFirstTimeUse4;

    private const string FirstTimeUsePersistentEvent = "CultTrialUsed";

    private Transform _startingLinePlacer;
    private Transform _startingLine;
    private Material _startingLineBaseMaterial;


    private CustomFogController _activeFogController;


    private void UpdateKeyframes()
    {
        _keyframes = transform.Find("Keyframes").GetComponentsInChildren<CultTrialKeyframe>().ToList();
        for (int i = 0; i < _keyframes.Count; i++)
        {
            _keyframes[i].isFinalKeyframe = i == _keyframes.Count - 1;
        }
        AlignStartingPosition();
    }

    private void AlignStartingPosition()
    {
        var placer = transform.Find("StartingLinePlacer");
        var t = transform.Find("StartingLine");
        
        if (Physics.Raycast(placer.position, Vector3.down, out var hit, 5f, ~LayerMask.GetMask("CultTrialStartingLine"),
                QueryTriggerInteraction.Ignore))
        {
            t.position = hit.point;
            GetComponentInChildren<Interactable>().transform.position = hit.point + Vector3.up;
            t.rotation = placer.rotation;
        }
    }

    private void OnInteracted()
    {
        Machine.Fire(CultTrialFsmTrigger.OnInteracted);
        
    }

    public void Unlock()
    {
        Machine.Fire(CultTrialFsmTrigger.OnUnlock);
    }

    private void AssumeActivation()
    {
        Machine.Jump(CultTrialFsmState.UnlockedIdle);
        _startingLineBaseMaterial.SetFloat("_RingMultiplier", 1f);
        EnableFlames();
    }

    private void EnableFlames()
    {
        var flamesParent = transform.Find("StartingLine").Find("Flames");
        for (int i = 0; i < flamesParent.childCount; i++)
        {
            var flame = flamesParent.GetChild(i);
            var light = flame.GetComponentInChildren<Light>();
            StartCoroutine(EnableAfterDelay(light,
                i * 0.2f));
        }

        return;

        IEnumerator EnableAfterDelay(Light light, float delay)
        {
            yield return new WaitForSeconds(delay);
            light.GetComponent<ParticleSystem>().Play();
            light.enabled = true;
            yield return null;
        }
    }

    private void OnDialogueCompleted()
    {
        Machine.Fire(CultTrialFsmTrigger.OnDialogueCompleted);
    }

    private void UpdateInteractable()
    {
        if (!SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent)) return;
        _interactable.text = CultTrialManager.Singleton.isCurseEnabled ? "Dispel mark" : "Accept mark";
    }
}