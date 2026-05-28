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

public class MajorLeylineNode : MonoBehaviour
{
    public string metaName;
    public string previousNodeMetaName;
    public float cameraSplineFollowDurationMultiplier = 1.0f;
    public bool cameraSpline = false;
    
    private SplineContainer _visualSplineContainer;
    private SplineContainer _cameraSplineContainer;
    private Interactable _mainInteractable;
    private Interactable _dialogueInteractable;
    private Material _visualSplineMaterial;

    // private Renderer _interactableHaloRenderer;
    // private Material _interactableHaloMaterial;
    private Renderer _completedHaloRenderer;
    private Material _completedHaloMaterial;
    private Renderer _channelHaloRenderer;
    private Material _channelHaloMaterial;
    private Material _nodeBaseMaterial;
    private Light _interactableLight;

    private Renderer _curvedStarRenderer;
    private Renderer _curvedStarOutlineRenderer;


    private ParticleSystem _channelParticles;
    private ParticleSystem _completedParticles;

    private CinemachineVirtualCamera _splineCamera;
    private Transform _cameraLookAt;
    private Transform _cameraFollow;
    private CinemachineVirtualCamera _finalCamera;

    private MinorCheckpoint _checkpoint;
    
    private Transform _nodeTransform;

    private bool _cameraSplineActive;

    private const float CameraSplineDistance = 15f;
    private List<TerminalColumn> _terminalColumns;
    

    private void Awake()
    {
        _visualSplineContainer = transform.Find("VisualSpline").GetComponent<SplineContainer>();
        _cameraSplineContainer = transform.Find("CameraSpline").GetComponent<SplineContainer>();

        _cameraLookAt = transform.Find("CameraLookAt");
        _cameraFollow = transform.Find("CameraFollow");
        _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(0);
        _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(0);

        _visualSplineMaterial = _visualSplineContainer.GetComponent<MeshRenderer>().material;
        _mainInteractable = transform.Find("Node").Find("Interactable").GetComponent<Interactable>();
        _dialogueInteractable = transform.Find("Node").Find("Dialogue").GetComponent<Interactable>();
        
        _splineCamera = transform.Find("MajorLeylineNodeSplineCamera").GetComponent<CinemachineVirtualCamera>();
        _finalCamera = transform.Find("Node").Find("FinalCameraHolder").GetComponentInChildren<CinemachineVirtualCamera>();
        _splineCamera.Priority = -20;
        _finalCamera.Priority = -20;

        _nodeTransform = transform.Find("Node");
        _completedHaloRenderer = transform.Find("Node").Find("CompletedHalo").GetComponent<Renderer>();
        _completedHaloMaterial = _completedHaloRenderer.material;
        
        _curvedStarRenderer = transform.Find("Node").Find("CurvedStar").GetComponent<Renderer>();
        _curvedStarOutlineRenderer = transform.Find("Node").Find("CurvedStarOutline").GetComponent<Renderer>();
        
        _channelHaloRenderer = transform.Find("Node").Find("ChannelHalo").GetComponent<Renderer>();
        _channelHaloMaterial = _channelHaloRenderer.material;
        _channelParticles = transform.Find("Node").Find("ChannelParticles").GetComponent<ParticleSystem>();
        _completedParticles = transform.Find("Node").Find("CompletedParticles").GetComponent<ParticleSystem>();
        _interactableLight = transform.Find("Node").Find("InteractableLight").GetComponent<Light>();
        
        _nodeBaseMaterial = transform.Find("Node").Find("major-leyline-node").Find("MajorLeylineNodeBase").GetComponent<Renderer>().material;
        _terminalColumns = GetComponentsInChildren<TerminalColumn>().ToList();

        _checkpoint = GetComponentInChildren<MinorCheckpoint>();

        _visualSplineMaterial.SetFloat("_FillWeight", 0f);
        _checkpoint.gameObject.SetActive(false);
        
        
        _curvedStarRenderer.enabled = false;
        _curvedStarOutlineRenderer.enabled = false;
        _cameraSplineActive = false;
        

        

        if (previousNodeMetaName == "" && !SaveSystem.GetMajorLeylineNode(metaName))
        {
            MakeInteractable();
            return;
        }

        if (SaveSystem.GetMajorLeylineNode(metaName))
        {
            MakeCompleted();
            return;
        }

        if (!SaveSystem.GetMajorLeylineNode(previousNodeMetaName))
        {
            MakeUninteractable();
            return;
        }
        
        MakeInteractable();
    }

    private void OnEnable()
    {
        _mainInteractable.OnInteracted += OnInteracted;
        SaveSystem.OnSaveDataUpdated += OnSaveDataUpdated;
        GlyphManager.MajorLeylineNodes.Add(this);
    }

    private void OnTutorialTrigger(Collider obj)
    {
        //
        // DialogueCanvas.Singleton.StartDialogue(_tutorialDialogueController);
        // PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
    }


    private void OnDisable()
    {
        _mainInteractable.OnInteracted -= OnInteracted;
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
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.WalkToMajorLeylinePosition);
        PlayerFsm.Singleton.walkToPositionTarget = _mainInteractable.transform.position;
        
        StartCoroutine(MainCoroutine());
        StartCoroutine(FovCoroutine());
        
        SaveSystem.WriteMajorLeylineNode(metaName);
        _mainInteractable.SetEnabled(false);

        IEnumerator MainCoroutine()
        {
            

            var cameraDirection = _nodeTransform.position - Camera.main.transform.position;
            PlayerCinemachineFreeLook.Singleton.OnPlayerCinemachineFreeLookScript(cameraDirection, 3f, 0.4f, 2f);
            
            // Wait for player to be in animation
            while (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.WalkToMajorLeylinePosition))
                yield return null;
            
            CutsceneManager.Singleton.SetPseudoCutsceneActive();
            
            yield return new WaitForSeconds(0.5f);
            
            // Animate halos
            var t = 0f;
            var d = 1.5f;
            var channelBaseScale = _channelHaloRenderer.transform.localScale;
            _channelParticles.Play();
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _channelHaloRenderer.transform.localScale = Vector3.Lerp(channelBaseScale, channelBaseScale * 0.8f, w);
                
                _channelHaloMaterial.SetFloat("_Dot", Mathf.Lerp(3f, 0, w));
                _channelHaloMaterial.SetFloat("_FresnelDepth", Mathf.Lerp(3f, 0, w)); 
                
                t += Time.deltaTime;
                yield return null;
            }
            
            // _interactableHaloRenderer.enabled = false;
            _channelHaloRenderer.enabled = false;
            _interactableLight.enabled = false;
            _channelParticles.Stop();
            
            if (cameraSpline) StartCoroutine(CameraSplineCoroutine());
            while (_cameraSplineActive) yield return null;
            
            yield return new WaitForSeconds(0.5f);
            
            _visualSplineMaterial.SetFloat("_FillWeight", 1f);
            _checkpoint.gameObject.SetActive(true);
            _dialogueInteractable.SetEnabled(true);
            ShowCurvedStarForDialogue();
            
            yield return new WaitForSeconds(1.0f);
            
            // Animate line glow on node base
            t = 0;
            d = 0.6f;
            
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _nodeBaseMaterial.SetFloat("_CompletionWeight", w);
                SetTerminalColumnWeights(w);
                t += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(1f);

            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
        }
        
        IEnumerator FovCoroutine()
        {
            
            var t = 0f;
            var d = 1f;

            var freeLook = PlayerCinemachineFreeLook.Singleton.GetFreeLook();
            var baseFov = PlayerCinemachineFreeLook.Singleton.GetBaseFov();
            var offset = freeLook.transform.GetComponent<CinemachineCameraOffset>();

            var desiredOffset = new Vector3(0, 0, -10f);
            var desiredFov = 60f;

            
            while (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.WalkToMajorLeylinePosition))
                yield return null;
            
            
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                
                offset.m_Offset = Vector3.Lerp(Vector3.zero, desiredOffset, w);
                freeLook.m_Lens.FieldOfView = Mathf.Lerp(baseFov, desiredFov, w);

                t += Time.deltaTime;
                yield return null;
            }

            while (!_checkpoint.gameObject.activeInHierarchy) yield return null;
            
            t = 0f;
            d = 10f;
            var s = 2f;
            // freeLook.m_Lens.FieldOfView = 85f;
            // offset.m_Offset = new Vector3(0, 0, 3f);
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                
                offset.m_Offset = Vector3.Lerp(offset.m_Offset, Vector3.zero, Time.deltaTime * s);
                freeLook.m_Lens.FieldOfView = Mathf.Lerp(freeLook.m_Lens.FieldOfView, baseFov, Time.deltaTime * s);
                t += Time.deltaTime;
                yield return null;
            }
            
            offset.m_Offset = Vector3.zero;
            freeLook.m_Lens.FieldOfView = baseFov;
        }

        IEnumerator CameraSplineCoroutine()
        {
            _cameraSplineActive = true;
            var splineLength = _cameraSplineContainer.Spline.GetLength();
            var startingT = (splineLength - CameraSplineDistance) / splineLength;
            CutsceneManager.Singleton.SetPseudoCutsceneActive(_cameraLookAt);
            _splineCamera.Priority = 30;

            var t = 0f;
            var d = 4f;

            while (t < d)
            {
                var fillWeight = Mathf.Lerp(startingT, 1f, t / d);
                _cameraLookAt.position = _visualSplineContainer.transform.TransformPoint(_visualSplineContainer.Spline.EvaluatePosition(fillWeight));
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

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnSaveDataUpdated(SaveSystem.SaveData saveData)
    {
        if (!_mainInteractable.isEnabled && !SaveSystem.GetMajorLeylineNode(metaName) &&
            SaveSystem.GetMajorLeylineNode(previousNodeMetaName))
        {
            MakeInteractable();
        }
    }

    private void MakeUninteractable()
    {
        _mainInteractable.SetEnabled(false);
        // _interactableHaloRenderer.enabled = false;
        _channelHaloRenderer.enabled = false;
        // _curvedStarMaterial.SetFloat("_Clip", 1);
        // _curvedStarOutlineMaterial.SetFloat("_Clip", 1);
        _interactableLight.enabled = false;
        _dialogueInteractable.SetEnabled(false);
    }

    private void MakeInteractable()
    {
        _mainInteractable.SetEnabled(true);
        // _curvedStarMaterial.SetFloat("_Clip", 0);
        // _curvedStarOutlineMaterial.SetFloat("_Clip", 0);
        _interactableLight.enabled = true;
        _channelHaloRenderer.enabled = true;
        _dialogueInteractable.SetEnabled(false);
    }

    private void MakeCompleted()
    {
        _mainInteractable.SetEnabled(false);
        _mainInteractable.enabled = false;
        _visualSplineMaterial.SetFloat("_FillWeight", 1f);
        // _interactableHaloRenderer.enabled = false;
        _channelHaloRenderer.enabled = false;
        // _curvedStarMaterial.SetFloat("_Clip", 1f);
        _interactableLight.enabled = false;
        _nodeBaseMaterial.SetFloat("_CompletionWeight", 1f);
        SetTerminalColumnWeights(1f);
        _checkpoint.gameObject.SetActive(true);
        _dialogueInteractable.SetEnabled(SaveSystem.GetMajorLeylineNodeDialogueLocation() == metaName);
        ShowCurvedStarForDialogue();
    }

    private void ShowCurvedStarForDialogue()
    {
        if (!_dialogueInteractable.isEnabled || !_dialogueInteractable.gameObject.activeInHierarchy) return;
        _curvedStarRenderer.enabled = true;
        _curvedStarOutlineRenderer.enabled = true;
    }

    public SplineContainer GetVisualSplineContaier()
    {
        return _visualSplineContainer;
    }

    private void SetTerminalColumnWeights(float w)
    {
        foreach (var tc in _terminalColumns)
        {
            tc.SetCompletionWeight(w);
        }
    }
}
