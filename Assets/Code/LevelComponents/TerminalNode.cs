using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Code.LevelComponents;
using Code.Misc;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class TerminalNode : MonoBehaviour
{
    public string metaName;
    public string previousNodeMetaName;
    public float cameraSplineFollowDurationMultiplier = 1.0f;
    public bool cameraSpline = false;
    
    private SplineContainer _visualSplineContainer;
    private SplineContainer _cameraSplineContainer;
    private Interactable _interactable;
    private DialogueController _dialogueController;
    private Material _visualSplineMaterial;
    public Renderer _arc1Renderer;

    
    private CinemachineVirtualCamera _splineCamera;
    private Transform _cameraLookAt;
    private Transform _cameraFollow;
    
    private Transform _nodeTransform;

    private bool _cameraSplineActive;

    private const float CameraSplineDistance = 20f;

    public float mapSplineTMultiplier = 1f;

    private bool _isActive;
    public Canvas _mainCanvas;
    

    private void Awake()
    {
        _visualSplineContainer = transform.Find("VisualSpline").GetComponent<SplineContainer>();
        _cameraSplineContainer = transform.Find("CameraSpline").GetComponent<SplineContainer>();

        _cameraLookAt = transform.Find("CameraLookAt");
        _cameraFollow = transform.Find("CameraFollow");
        _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(0);
        _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(0);

        _visualSplineMaterial = _visualSplineContainer.GetComponent<MeshRenderer>().material;
        _interactable = transform.Find("NodeRedux").Find("Interactable").GetComponent<Interactable>();
        _dialogueController = GetComponentInChildren<DialogueController>();
        
        _splineCamera = transform.Find("MajorLeylineNodeSplineCamera").GetComponent<CinemachineVirtualCamera>();
        _splineCamera.Priority = -20;

        _nodeTransform = transform.Find("NodeRedux");
        
        _visualSplineMaterial.SetFloat("_FillWeight", 0f);
        _cameraSplineActive = false;
        _isActive = false;
        
        
        MakeReady();

    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
        _dialogueController.OnCompleted += OnDialogueCompleted;
        SaveSystem.OnSaveDataUpdated += OnSaveDataUpdated;
        GlyphManager.MajorLeylineNodes.Add(this);
    }

    private void OnDialogueCompleted()
    {
        IEnumerator Coroutine()
        {
            var t = 0f;
            var d = 0.6f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                t += Time.deltaTime;
                _mainCanvas.transform.localScale = Vector3.Lerp(new Vector3(1f, 1f, 1f), new Vector3(1f, 0f, 1f), w);
                yield return null;
            }
            MakeCompleted();
        }
        StartCoroutine(Coroutine());
    }

    private void OnSaveDataUpdated(SaveSystem.SaveData obj)
    {
        
    }


    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        _dialogueController.OnCompleted -= OnDialogueCompleted;
        SaveSystem.OnSaveDataUpdated -= OnSaveDataUpdated;
        GlyphManager.MajorLeylineNodes.Remove(this);
    }

    void Start()
    {
        
        var curveLength = _visualSplineContainer.Spline.GetLength();
        _visualSplineMaterial.SetFloat("_SplineLength", curveLength);
        
        var startingT = (curveLength - CameraSplineDistance) / curveLength;
        _cameraLookAt.position = _visualSplineContainer.transform.TransformPoint(_visualSplineContainer.Spline.EvaluatePosition(startingT));
        _cameraFollow.position = _cameraSplineContainer.transform.TransformPoint(_cameraSplineContainer.Spline.EvaluatePosition(0));
        
        
    }

    void OnInteracted()
    {
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.WalkToTerminalNodePosition);
        PlayerFsm.Singleton.walkToPositionTarget = _interactable.transform.position;
        
        StartCoroutine(MainCoroutine());


        var current = metaName;
        while (current != "")
        {
            SaveSystem.WriteTerminalNode(current);
            current = GlyphManager.TerminalRegistry[current].previousNode;
        }

        IEnumerator MainCoroutine()
        {
            
            // Wait for player to be in animation
            while (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.WalkToTerminalNodePosition))
                yield return null;
            
            float t;
            float d;
            
            if (cameraSpline) StartCoroutine(CameraSplineCoroutine());
            while (_cameraSplineActive) yield return null;
            
            _visualSplineMaterial.SetFloat("_FillWeight", 1f);
            _arc1Renderer.material.SetFloat("_GlowWeight", 1f);
            
 
            
            t = 0;
            d = 0.6f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                t += Time.deltaTime;
                _mainCanvas.transform.localScale = Vector3.Lerp(new Vector3(1f, 0f, 1f), new Vector3(1f, 1f, 1f), w);
                yield return null;
            }
            
            DialogueCanvas.Singleton.StartDialogue(_dialogueController);
            PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
            
        }
        
        
        IEnumerator CameraSplineCoroutine()
        {
            _cameraSplineActive = true;
            yield return new WaitForSeconds(1f);
            var splineLength = _cameraSplineContainer.Spline.GetLength();
            var startingT = (splineLength - CameraSplineDistance) / splineLength;
            _splineCamera.Priority = 30;

            var t = 0f;
            var d = 4f;

            while (t < d)
            {
                var fillWeight = Mathf.Lerp(startingT, 1f, t / d);
                // _cameraLookAt.position = _visualSplineContainer.transform.TransformPoint(_visualSplineContainer.Spline.EvaluatePosition(fillWeight));
                _visualSplineMaterial.SetFloat("_FillWeight", fillWeight);
                _cameraFollow.position = _cameraSplineContainer.transform.TransformPoint(_cameraSplineContainer.Spline.EvaluatePosition(Util.SmoothLerp01(t / d * 0.75f)));
                t += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            
            _splineCamera.Priority = -30;
            _cameraSplineActive = false;
        }
    }
    
    

    public SplineContainer GetVisualSplineContaier()
    {
        return _visualSplineContainer;
    }

    private void MakeReady()
    {
        _mainCanvas.transform.localScale = new Vector3(1f, 0f, 1f);
    }
    
    private void MakeCompleted()
    {
        _mainCanvas.transform.localScale = new Vector3(1f, 0f, 1f);
        _arc1Renderer.material.SetFloat("_GlowWeight", 1f);
    }
    
}
