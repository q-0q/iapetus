using System;
using System.Collections;
using Cinemachine;
using Code.Misc;
using UnityEngine;
using UnityEngine.Splines;

public class MajorLeylineNode : MonoBehaviour
{
    public string metaName;
    public string previousNodeMetaName;
    
    private SplineContainer _visualSplineContainer;
    private SplineContainer _cameraSplineContainer;
    private Interactable _interactable;
    private Material _visualSplineMaterial;

    private Renderer _interactableHaloRenderer;
    private Material _interactableHaloMaterial;
    private Renderer _completedHaloRenderer;
    private Material _completedHaloMaterial;
    private Renderer _channelHaloRenderer;
    private Material _channelHaloMaterial;

    private ParticleSystem _channelParticles;
    private ParticleSystem _completedParticles;

    private CinemachineVirtualCamera _virtualCamera;
    private Transform _cameraLookAt;
    private Transform _cameraFollow;

    private void Awake()
    {
        _visualSplineContainer = transform.Find("VisualSpline").GetComponent<SplineContainer>();
        _cameraSplineContainer = transform.Find("CameraSpline").GetComponent<SplineContainer>();

        _cameraLookAt = transform.Find("CameraLookAt");
        _cameraFollow = transform.Find("CameraFollow");
        _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(0);
        _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(0);

        _visualSplineMaterial = _visualSplineContainer.GetComponent<MeshRenderer>().material;
        _interactable = GetComponentInChildren<Interactable>();
        
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _virtualCamera.Priority = -20;

        _interactableHaloRenderer = transform.Find("Node").Find("InteractableHalo").GetComponent<Renderer>();
        _interactableHaloMaterial = _interactableHaloRenderer.material;
        _completedHaloRenderer = transform.Find("Node").Find("CompletedHalo").GetComponent<Renderer>();
        _completedHaloMaterial = _completedHaloRenderer.material;
        
        _channelHaloRenderer = transform.Find("Node").Find("ChannelHalo").GetComponent<Renderer>();
        _channelHaloMaterial = _channelHaloRenderer.material;
        _channelParticles = transform.Find("Node").Find("ChannelParticles").GetComponent<ParticleSystem>();
        _completedParticles = transform.Find("Node").Find("CompletedParticles").GetComponent<ParticleSystem>();

        if (previousNodeMetaName == "")
        {
            // _visualSplineContainer.gameObject.SetActive(false);
            _interactable.SetEnabled(true);
        }
        else
        {
            _interactable.SetEnabled(SaveSystem.GetMajorLeylineNode(previousNodeMetaName));
        }
        
        if (SaveSystem.GetMajorLeylineNode(metaName))
        {
            _interactable.SetEnabled(false);
            _visualSplineMaterial.SetFloat("_FillWeight", 1f);
        }
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
        SaveSystem.OnSaveDataUpdated += OnSaveDataUpdated;
    }



    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        SaveSystem.OnSaveDataUpdated -= OnSaveDataUpdated;
    }

    void Start()
    {
        
        var curveLength = _visualSplineContainer.Spline.GetLength();
        _visualSplineMaterial.SetFloat("_SplineLength", curveLength);
        
    }

    void OnInteracted()
    {
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.WalkToMajorLeylinePosition);
        PlayerFsm.Singleton.walkToPositionTarget = _interactable.transform.position;
        
        StartCoroutine(MainCoroutine());
        StartCoroutine(FovCoroutine());
        
        // SaveSystem.WriteMajorLeylineNode(metaName);
        _interactable.SetEnabled(false);

        IEnumerator MainCoroutine()
        {
            var t = 0f;
            var d = 0.5f;
            var channelBaseScale = _channelHaloRenderer.transform.localScale;

            while (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.WalkToMajorLeylinePosition))
                yield return null;

            _channelParticles.Play();

            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _channelHaloRenderer.transform.localScale = Vector3.Lerp(channelBaseScale, channelBaseScale * 0.8f, w);
                
                _channelHaloMaterial.SetFloat("_Dot", Mathf.Lerp(2f, 0, w));
                _channelHaloMaterial.SetFloat("_FresnelDepth", Mathf.Lerp(3f, 0, w));
                _interactableHaloMaterial.SetFloat("_Dot", Mathf.Lerp(1.5f, 0, w));
                _interactableHaloMaterial.SetFloat("_FresnelDepth", Mathf.Lerp(2f, 0, w));
                t += Time.deltaTime;
                yield return null;
            }

            _interactableHaloRenderer.enabled = false;
            yield return new WaitForSeconds(0.9f);
            _completedParticles.Play();
            yield return new WaitForSeconds(1.1f);
            CutsceneManager.Singleton.SetPseudoCutsceneActive(true, _cameraLookAt);
            yield return new WaitForSeconds(1f);
            
            
            t = 0f;
            d = _visualSplineContainer.Spline.GetLength() * 0.03f;
            _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(0);
            _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(0);
            _virtualCamera.Priority = 20;

            _channelParticles.Stop();
            _completedParticles.Stop();
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(w);
                _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(w);
                _visualSplineMaterial.SetFloat("_FillWeight", w);
                t += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(0.4f);
            
            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _virtualCamera.Priority = -20;
        }
        
        IEnumerator FovCoroutine()
        {
            var t = 0f;
            var d = 1.5f;

            var freeLook = PlayerCinemachineFreeLook.Singleton.GetFreeLook();
            var baseFov = PlayerCinemachineFreeLook.Singleton.GetBaseFov();
            var offset = freeLook.transform.GetComponent<CinemachineCameraOffset>();

            var desiredOffset = new Vector3(0, 0, -8f);
            var desiredFov = 65f;

            
            
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                
                offset.m_Offset = Vector3.Lerp(Vector3.zero, desiredOffset, w);
                freeLook.m_Lens.FieldOfView = Mathf.Lerp(baseFov, desiredFov, w);

                t += Time.deltaTime;
                yield return null;
            }

            _channelHaloRenderer.enabled = false;
            
            t = 0f;
            d = 0.5f;
            
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                
                offset.m_Offset = Vector3.Lerp(desiredOffset, Vector3.zero, w);
                freeLook.m_Lens.FieldOfView = Mathf.Lerp(desiredFov, baseFov, w);
                t += Time.deltaTime;
                yield return null;
            }
            
            offset.m_Offset = Vector3.zero;
            freeLook.m_Lens.FieldOfView = baseFov;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnSaveDataUpdated(SaveSystem.SaveData saveData)
    {
        if (!_interactable.isEnabled && !SaveSystem.GetMajorLeylineNode(metaName) &&
            SaveSystem.GetMajorLeylineNode(previousNodeMetaName))
        {
            _interactable.SetEnabled(true);
        }
    }
}
