using System;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class CrabPassageCutsceneFsm
{
    public DialogueController warning1;
    public DialogueController warning2;

    public TriggerProxy CutsceneTrigger1;
    public TriggerProxy CutsceneTrigger2;
    public TriggerProxy CutsceneTrigger3;

    
    
    private void OnDialogueProgressed(int textIndex)
    {

    }


}