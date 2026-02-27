using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.Misc;
using UnityEngine;

public class BellDoorController : MonoBehaviour
{

    private Animator _animator;
    private CinemachineVirtualCamera _passiveCamera;
    public static event Action OnPlayerNearbyUnopenedBellDoor;
    public string persistentEvent;
    private bool _opened;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _passiveCamera = transform.Find("PassiveCamera").GetComponentInChildren<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Util.ReplaceAnimatorTrigger(_animator, "Open");
        }
        
        if (_passiveCamera.Priority > 0) OnPlayerNearbyUnopenedBellDoor?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        _passiveCamera.Priority = 20;
    }

    private void OnTriggerExit(Collider other)
    {
        _passiveCamera.Priority = -20;
    }
}
