using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    
    private PlayerInput _playerInput;
    private CanvasGroup _canvasGroup;
    private Image _closeImage;
    private MapControllerState _state;
    private Transform _playerMarkerTransform;
    private Transform _mapObject;
    
    enum MapControllerState
    {
        Closed,
        Main,
        UseConfirmation
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponentInChildren<CanvasGroup>();

        _canvasGroup.alpha = 0;
        
        _closeImage = _canvasGroup.transform.Find("CloseInput").Find("Image").GetComponent<Image>();
        _mapObject = transform.Find("MapObject");
        
        _playerMarkerTransform = _mapObject.Find("Rotator").Find("PlayerMarker");
        
        _mapObject.SetParent(Camera.main.transform);
        _mapObject.transform.localRotation = Quaternion.identity;
        _mapObject.transform.localPosition = new Vector3(0f, 0f, 1.25f);
    }

    private void OnEnable()
    {
        PlayerFsm.PlayerMapEntered += OpenDelay;
        PlayerFsm.PlayerMapExited += Close;
        GameMenu.OnGameMenuOpened += OnGameMenuOpened;
        GameMenu.OnGameMenuClosed += OnGameMenuClosed;
    }

    private void OpenDelay()
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(0.25f);
            Open();
        }
    }

    private void OnDisable()
    {
        PlayerFsm.PlayerMapEntered -= OpenDelay;
        PlayerFsm.PlayerMapExited -= Close;
        GameMenu.OnGameMenuOpened -= OnGameMenuOpened;
        GameMenu.OnGameMenuClosed -= OnGameMenuClosed;
    }

    // Update is called once per frame
    void LateUpdate()
    {


        if (_playerInput.actions["Look"].ReadValue<Vector2>().magnitude > 0.01 &&
            InputTypeManager.Singleton.GetCurrentInputType() == InputTypeManager.InputType.Kmb && GetIsOpen())
        {
            Cursor.visible = true;  
            Cursor.lockState = CursorLockMode.None;
        }
        
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, GetIsOpen() ? 1f : 0f, Time.deltaTime * 20f);
        _closeImage.sprite = InputTypeManager.Singleton.GetSpriteForAction("Map");

        if (_state == MapControllerState.Main)
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1f, Time.deltaTime * 20f);
        }
        
        
        
    }

    private void LerpToTransformToCamera(float speed)
    {
        var destPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
        var destRot = Camera.main.transform.rotation;
        
        _mapObject.transform.position = speed > 0 ? Vector3.Lerp(_mapObject.transform.position, destPos, Time.deltaTime * speed) :  destPos;
        _mapObject.transform.rotation = speed > 0 ? Quaternion.Lerp(_mapObject.transform.rotation, destRot, Time.deltaTime * speed) : destRot;
    }

    void Open()
    {
        _mapObject.gameObject.SetActive(true);

        _state = MapControllerState.Main;
        _canvasGroup.blocksRaycasts = true;
        
        ComputePlayerMapPosition();
        
        _mapObject.transform.DOComplete();
        // _mapObject.transform.DOPunchRotation(Vector3.forward * 2f, 0.15f, 20, 1f);
        _mapObject.transform.DOPunchScale(Vector3.one * -0.1f, 0.4f, 5, 1f);
        
        if (TutorialCanvas.Singleton.GetCurrentAction() == "Map") TutorialCanvas.Singleton.HideTutorialText();


        var currentNearestMajorLeylineNode = SaveSystem.GetNearestTerminalNode(out var currentMajorLeylineNodeT);
        var terminalData = GlyphController.TerminalRegistry[currentNearestMajorLeylineNode];
        if (terminalData == null) return;
        var splines = _mapObject.GetComponentInChildren<GlyphController>().GetSplines();
        if (splines == null) return;
        var splineContainer = splines[terminalData.mapSplineId];
        if (splineContainer == null) return;
        var clampedLocalT = Mathf.Clamp01(currentMajorLeylineNodeT);
        var t = Mathf.Lerp(terminalData.mapSplineStartT, terminalData.mapSplineEndT, clampedLocalT);
        _playerMarkerTransform.position = splineContainer.GetComponent<SplineContainer>().EvaluatePosition(t);
        
    }
    
    void Close()
    {
        _mapObject.gameObject.SetActive(false);
        _state = MapControllerState.Closed;
        
        _canvasGroup.blocksRaycasts = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    private void OnGameMenuOpened()
    {
        if (!GetIsOpen()) return;
        

        
        
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
    
    private void OnGameMenuClosed()
    {
        if (!GetIsOpen()) return;
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }
    
    public bool GetIsOpen()
    {
        return _state != MapControllerState.Closed;
    }

    private void Start()
    {
        Close();
    }
    
    private void ComputePlayerMapPosition()
    {
        if (GlyphController.TerminalsInScene.Count == 0) return;

        var playerPosition = PlayerFsm.Singleton.transform.position;
        
        var nearestDistance = Mathf.Infinity;
        var nearestNode = "";
        var nearestT = 0f;

        foreach (var majorLeylineNode in GlyphController.TerminalsInScene)
        {
            print(majorLeylineNode.name);
            var visualSpline = majorLeylineNode.GetVisualSplineContainer(out _);
            SplineUtility.GetNearestPoint(visualSpline.Spline, visualSpline.transform.InverseTransformPoint(playerPosition), out var nearest, out var t);
            var vector3 = new Vector3(nearest.x, nearest.y, nearest.z);
            vector3 = visualSpline.transform.TransformPoint(vector3);
            var d = Vector3.SqrMagnitude(vector3 - playerPosition);

            if (d < nearestDistance)
            {
                nearestDistance = d;
                nearestNode = majorLeylineNode.metaName;
                nearestT = Mathf.Lerp(majorLeylineNode.mapSplineTMin, majorLeylineNode.mapSplineTMax, Mathf.Clamp01(t));
            }
        }
        
        SaveSystem.WriteNearestTerminalNode(nearestNode, nearestT);
    }
}
