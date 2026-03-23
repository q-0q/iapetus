using System;
using System.Reflection;

using System.Collections;
using System.Collections.Generic;
using Code.TriggerParams;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


public class TutorialTrigger : MonoBehaviour
{
    public string action = "";
    public string text = "Tutorial text";
    public string playerHideState = "";
    private int playerHideStateInt = -1;
    private bool active = false;

    private void Start()
    {
        FieldInfo field = typeof(PlayerFsm.PlayerFsmState).GetField(
            playerHideState,
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (field != null)
        {
            playerHideStateInt = (int)field.GetValue(null);
        }
    }

    private void Update()
    {
        if (active && PlayerFsm.Singleton.Machine.IsInState(playerHideStateInt))
        {
            TutorialCanvas.Singleton.HideTutorialText();
            active = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerFsm.Singleton.Machine.IsInState(playerHideStateInt)) return;
        TutorialCanvas.Singleton.ShowTutorialText(text, action);
        active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        TutorialCanvas.Singleton.HideTutorialText();
        active = false;
    }
}
