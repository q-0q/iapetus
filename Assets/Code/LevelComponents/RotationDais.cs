using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationDais : MonoBehaviour
{

    private Interactable _interactable;
    private bool _active;
    private Transform _rotator;
    public Transform _ball;
    private PlayerInput _playerInput;

    private Vector3 _baseBallWorldPosition;

    private string _baseInteractableText;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        _playerInput = GetComponent<PlayerInput>();
        _rotator = transform.Find("Rotator");
        _baseInteractableText = _interactable.text;
        _baseBallWorldPosition = _ball.transform.position;
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
            PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Idle);
            _active = false;
            _interactable.text = _baseInteractableText;
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
        Vector3 ballWorldSpaceOffset = new Vector3(0f, Mathf.Sin(3f * Time.time) * 0.1f, 0f);
        _ball.position = _baseBallWorldPosition + ballWorldSpaceOffset;
    }
}
