using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class Dialogue
{
    public List<string> texts = new List<string>();
}

public class DialogueController : MonoBehaviour
{
    public int currentDialogueIndex = 0;
    public int textStartOffset = 0;
    
    public List<Dialogue> dialogues;
    public string DialogueName = "Unnamed dialogue";
    public event Action OnCompleted;
    private void OnEnable()
    {
        TryGetComponent(out Interactable interactable);
        interactable.OnInteracted += StartDialogue;
    }

    private void StartDialogue()
    {
        DialogueCanvas.Singleton.StartDialogue(this);
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.StartDialogue);

    }

    private void OnDisable()
    {
        TryGetComponent(out Interactable interactable);
        interactable.OnInteracted -= StartDialogue;
    }

    public void Completed()
    {
        Debug.Log("completed dialogue");
        OnCompleted?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
