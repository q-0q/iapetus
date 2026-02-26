using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class OnetimeSwitchFsm
{
    private Interactable _interactable;
    private PowerConnector _powerConnector;
    public CinemachineVirtualCamera _VirtualCamera;
    
    public static Action<OnetimeSwitchFsm> OnOnetimeSwitchFsmTurnedOn;

    private void StartPlayerInteraction()
    {
        InteractableParam p = new InteractableParam() { Interactable = _interactable, WalkToPositionTarget =
            _interactable.transform.position};
        PlayerFsm.Singleton.Machine.Fire(PlayerFsm.PlayerFsmTrigger.InteractWithSwitch, p);
    }

    private void InvokeVirtualCamera()
    {
        if (_VirtualCamera == null) return;

        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            CutsceneManager.Singleton.SetPseudoCutsceneActive();
            yield return new WaitForSeconds(0.75f);
            float t = 0;
            float duration = 1.25f;
            while (t < duration)
            {
                _VirtualCamera.Priority = 20;
                yield return null;
                t += Time.deltaTime;
            }
            
            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _VirtualCamera.Priority = -10;
            yield return null;
        }
    }
}