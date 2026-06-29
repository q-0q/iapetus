using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PhotoManager : MonoBehaviour
{

    public event Action OnPhotoModeActivated;
    public event Action OnPhotoModeDeactivated;
    
    public static PhotoManager Singleton;
    private bool _active;
    
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFreeLook _playerFreeLook;
    private const float ActiveTimescale = 0.00001f;
    private const float MaxTranslationSpeed = 30f;
    private const float TranslationAcceleration = 50f;
    private Vector3 _currentTranslationVelocity;
    
    private PlayerInput _playerInput;

    private CanvasGroup _canvasGroup;
    private bool _isCanvasVisible;
    private bool _isAdvancing;

    public Image moveImage;
    public Image turnImage;
    public Image raiseImage;
    public Image lowerImage;
    public Image advanceImage;
    public Image hideDisplayImage;
    public Image exitImage;

    private void Awake()
    {
        Singleton = this;
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _canvasGroup.alpha = 0;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerFreeLook = PlayerCinemachineFreeLook.Singleton.GetFreeLook();
        MakeInactive();
    }
    
    void Update()
    {
        if (!_active) return;

        if (_playerInput.actions["Interact"].WasPressedThisFrame()) _isCanvasVisible = !_isCanvasVisible;
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _isCanvasVisible ? 1f : 0f, Time.unscaledDeltaTime * 20f);

        moveImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Move");
        turnImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Look");
        raiseImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Jump");
        lowerImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Sprint");
        advanceImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Inventory");
        hideDisplayImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Interact");
        exitImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Camera");
        
        DoTranslation();
        DoRotation();
        DoAdvanceGame();
    }

    public void MakeActive()
    {
        _virtualCamera.transform.position = Camera.main.transform.position;

        var pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        var pitch = Camera.main.transform.rotation.eulerAngles.y;
        var yaw = Camera.main.transform.localRotation.eulerAngles.x;
        // if (pitch > 180f) pitch -= 360f;
        // if (yaw > 180f) pitch -= 360f;
        pov.m_HorizontalAxis.Value = pitch;
        pov.m_VerticalAxis.Value = yaw;
        
        _virtualCamera.m_Lens.FieldOfView = _playerFreeLook.m_Lens.FieldOfView;
        _virtualCamera.Priority = 50;
            
        _virtualCamera.UpdateCameraState(Vector3.up, Time.unscaledDeltaTime);
        _currentTranslationVelocity = Vector3.zero;
        
        _canvasGroup.alpha = 1;
        _isCanvasVisible = true;
        var timeScale = ActiveTimescale;
        Time.timeScale = timeScale;
        _active = true;
        OnPhotoModeActivated?.Invoke();
    }

    public void MakeInactive()
    {
        _canvasGroup.alpha = 0;
        _isCanvasVisible = false;
        Time.timeScale = 1f;
        _virtualCamera.Priority = -20;
        _active = false;
        OnPhotoModeDeactivated?.Invoke();
    }

    private void DoTranslation()
    {
        var v2 = _playerInput.actions["Move"].ReadValue<Vector2>();
        var v3 = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * new Vector3(v2.x, 0, v2.y);
        if (_playerInput.actions["Jump"].IsPressed()) v3 += Vector3.up;
        if (_playerInput.actions["Sprint"].IsPressed()) v3 -= Vector3.up;

        var targetTranslationVelocity = v3.normalized * MaxTranslationSpeed;

        var acceleration = _currentTranslationVelocity.magnitude < targetTranslationVelocity.magnitude
            ? TranslationAcceleration
            : TranslationAcceleration * 3f;
    
        _currentTranslationVelocity = Vector3.MoveTowards(
            _currentTranslationVelocity, 
            targetTranslationVelocity, 
            acceleration * Time.unscaledDeltaTime
        );
        
        _virtualCamera.transform.position += _currentTranslationVelocity * Time.unscaledDeltaTime;
    }
    
    private void DoRotation()
    {
        var v2 = _playerInput.actions["Look"].ReadValue<Vector2>() * 0.001f;
        
        var pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        pov.m_HorizontalAxis.m_InputAxisValue = v2.x * Time.unscaledDeltaTime;
        pov.m_VerticalAxis.m_InputAxisValue = v2.y * Time.unscaledDeltaTime;
        
    }

    private void DoAdvanceGame()
    {
        if (_isAdvancing) return;
        if (!_playerInput.actions["Inventory"].WasPressedThisFrame()) return;
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            _isAdvancing = true;
            Time.timeScale = 1f;
            yield return new WaitForSecondsRealtime(0.025f);
            Time.timeScale = ActiveTimescale;
            _isAdvancing = false;
        }
    }

    public bool IsActive()
    {
        return _active;
    }
    
}
