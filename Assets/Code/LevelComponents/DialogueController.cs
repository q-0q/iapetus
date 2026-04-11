using System;
using System.Collections;
using System.Collections.Generic;
using Code.TriggerParams;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Dialogue
{
    public List<string> texts = new List<string>();
    public bool advanceDialogueIndex = false;
}

public class DialogueController : MonoBehaviour
{
    public int currentDialogueIndex = 0;
    public int textStartOffset = 0;
    
    public List<Dialogue> dialogues;
    public string DialogueName = "Unnamed dialogue";
    private Interactable _interactable;
    public event Action OnCompleted;
    public event Action<int> OnProgressed;
    public Transform LookAtOverride;

    public float CameraY = 0.7f;

    public float canvasDelay = 0f;
    
    private void OnEnable()
    {
        TryGetComponent(out Interactable interactable);
        _interactable = interactable;
        _interactable.OnInteracted += StartDialogue;
    }

    private void StartDialogue()
    {
        InteractableParam p = new InteractableParam() { Interactable = _interactable, WalkToPositionTarget =
            transform.position};
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.StartDialogue, p);

        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(canvasDelay);
            DialogueCanvas.Singleton.StartDialogue(this);
        }
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= StartDialogue;
    }

    public void Completed()
    {
        if (dialogues[currentDialogueIndex].advanceDialogueIndex) currentDialogueIndex++;
        OnCompleted?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.WalkToDialoguePosition) &&
            !PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dialogue))
        {
            DialogueCanvas.Singleton.EndDialogue();
        }
    }

    public void ProgressionSignal(int textIndex)
    {
        OnProgressed?.Invoke(textIndex);
    }
}
