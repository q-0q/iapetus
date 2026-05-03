using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract partial class CultistFsm
{
    protected Interactable Interactable;
    protected DialogueController DialogueController;
    public Renderer circletRenderer;
    public Renderer robeDetailRenderer;
    private float _turnAmount;
    protected int CampId;

    protected virtual void OnDialogueProgressed(int textIndex)
    {

    }
}