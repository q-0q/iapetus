using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public class TeleportationInvoker : MonoBehaviour
{

    public string destinationPositionId;
    private Transform destinationTransform;
    private float destinationCameraRotationOffset;
    private Interactable _interactable;


    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        destinationTransform = Util.FindGamePositionById(destinationPositionId, out destinationCameraRotationOffset);
        if (destinationTransform == null)
        {
            Debug.LogError("teleport invoker \"" + name + "\" could not locate position id \"" + destinationPositionId + "\"");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnInteracted()
    {
        PlayerFsm.Singleton.SetTeleportDestination(destinationTransform.position, Quaternion.Euler(0, destinationCameraRotationOffset, 0) * destinationTransform.forward);
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.TrialTeleport);
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }
}
