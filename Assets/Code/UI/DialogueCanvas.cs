using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueCanvas : MonoBehaviour
{
    
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _tmpText;
    private TextMeshProUGUI _tmpName;
    public static DialogueCanvas Singleton;
    public DialogueController currentDialogueController;
    private int _currentTextIndex = 0;

    private void Awake()
    {
        Singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _tmpText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _tmpName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        _canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentDialogueController != null && !GameMenu.Singleton.IsMenuOpen())
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, Time.unscaledDeltaTime * 15f);
            _tmpText.text = currentDialogueController.dialogues[currentDialogueController.currentDialogueIndex].texts[_currentTextIndex + currentDialogueController.textStartOffset];
            _tmpName.text = currentDialogueController.DialogueName;
        }
        else
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0, Time.unscaledDeltaTime * 40f);
        }
    }

    public void AdvanceDialogue()
    {
        _currentTextIndex++;
        if (currentDialogueController != null && _currentTextIndex + currentDialogueController.textStartOffset >= currentDialogueController.dialogues[currentDialogueController.currentDialogueIndex].texts.Count)
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        if (currentDialogueController is null) return;
        currentDialogueController.Completed();
        currentDialogueController = null;
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.EndDialogue);
    }

    public void StartDialogue(DialogueController controller)
    {
        currentDialogueController = controller;
        _currentTextIndex = 0;
    }

    public Vector3 ControllerPosition()
    {
        return currentDialogueController.transform.position;
    }
}
