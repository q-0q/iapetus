using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class SwitchFsm
{
    private Interactable _interactable;
    private PowerConnector _powerConnector;
    public CinemachineVirtualCamera _VirtualCamera;

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
            yield return new WaitForSeconds(0.25f);
            CutsceneManager.Singleton.SetPseudoCutsceneActive();
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