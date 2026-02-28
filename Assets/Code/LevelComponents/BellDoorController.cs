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
    private float _playerLookAwayTimer;
    private const float LookawayTime = 0.75f;

    private void Awake()
    {
        _playerLookAwayTimer = 0f;
        _animator = GetComponentInChildren<Animator>();
        _passiveCamera = transform.Find("BellDoorPassiveCamera").GetComponentInChildren<CinemachineVirtualCamera>();
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

        if (Vector3.Angle(PlayerFsm.Singleton.transform.forward, transform.forward) < 90f)
        {
            _playerLookAwayTimer += Time.deltaTime;
        }
        else
        {
            _playerLookAwayTimer = 0;
        }
        
        if (_passiveCamera.Priority > 0) OnPlayerNearbyUnopenedBellDoor?.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        _passiveCamera.Priority = _playerLookAwayTimer >= LookawayTime ? -20 : 20;
    }

    private void OnTriggerExit(Collider other)
    {
        _passiveCamera.Priority = -20;
    }
}
