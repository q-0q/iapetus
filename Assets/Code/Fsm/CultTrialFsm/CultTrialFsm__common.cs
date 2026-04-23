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
using Random = Unity.Mathematics.Random;

public partial class CultTrialFsm
{
    public string metaName = "";
    
    private Interactable _interactable;
    private List<CultTrialKeyframe> _keyframes;

    private DialogueController _dialogueNoItem;
    private DialogueController _dialogueItem;
    private DialogueController _dialogueFirstTimeUse;

    private const string FirstTimeUsePersistentEvent = "CultTrialUsed";

    private Material _startingLineBaseMaterial;
    
    
    
    

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
        var controller = _dialogueNoItem;
        if (!SaveSystem.GetPersistentEventCompleted(metaName + "-unlocked"))
        {
            controller = SaveSystem.GetAllItems().Contains("IncenseBurner") ? _dialogueItem : _dialogueNoItem;
        }
        else
        {
            controller = SaveSystem.GetPersistentEventCompleted(FirstTimeUsePersistentEvent) ? null : _dialogueFirstTimeUse;
        }
        DialogueCanvas.Singleton.StartDialogue(controller);
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
    }

    public void DoActivation()
    {
        StartCoroutine(RingLightMultiplierCoroutine());
        EnableFlames();
        IEnumerator RingLightMultiplierCoroutine()
        {
            float t = 0f;
            float d = 0.25f;
            while (t < d)
            {
                _startingLineBaseMaterial.SetFloat("_RingMultiplier", Util.SmoothLerp01( t / d));
                t += Time.deltaTime;
                yield return null;
            }
        }
        
    }

    private void AssumeActivation()
    {
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
}