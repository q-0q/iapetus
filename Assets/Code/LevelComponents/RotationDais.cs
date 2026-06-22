using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationDais : MonoBehaviour
{

    private Interactable _interactable;
    private bool _active;
    private Transform _rotator;
    private Transform _ball;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        _playerInput = GetComponent<PlayerInput>();
        _rotator = transform.Find("Rotator");
        _ball = transform.Find("Dais").Find("Ball");
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    private void OnInteracted()
    {
        if (_active)
        {
            PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.IdleLong);
            _active = false;
            _interactable.text = "Interact";
            return;
        }
        
        _interactable.text = "Leave";
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.WalkToRotationDaisPosition);
        PlayerFsm.Singleton.walkToPositionTarget = _interactable.transform.position;
        _active = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    private Vector3 _currentRotationVelocity = Vector3.zero;

    void Update()
    {
        Vector2 input = _active && PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.RotationDaisInteract) 
            ? _playerInput.actions["Move"].ReadValue<Vector2>() : Vector3.zero;
        
        Vector3 right = transform.right;
        Vector3 up = -transform.up; 
        Vector3 targetAxis = (up * input.x) + (right * -input.y);
        
        float acceleration = 1.5f;
        float maxSpeed = 100f;
        
        _currentRotationVelocity = Vector3.MoveTowards(
            _currentRotationVelocity, 
            targetAxis, 
            acceleration * Time.deltaTime
        );
        
        _rotator.Rotate(_currentRotationVelocity * (maxSpeed * Time.deltaTime), Space.World);
        _ball.Rotate(_currentRotationVelocity * (maxSpeed * Time.deltaTime * 3f), Space.World);
    }
}
