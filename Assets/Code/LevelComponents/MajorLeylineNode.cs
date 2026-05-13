using System;
using System.Collections;
using Cinemachine;
using Code.Misc;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class MajorLeylineNode : MonoBehaviour
{
    public string metaName;
    public string previousNodeMetaName;
    public float cameraSplineFollowDurationMultiplier = 1.0f;
    
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
    private Material _nodeBaseMaterial;
    private Light _interactableLight;
    private Material _curvedStarMaterial;

    private ParticleSystem _channelParticles;
    private ParticleSystem _completedParticles;

    private CinemachineVirtualCamera _virtualCamera;
    private Transform _cameraLookAt;
    private Transform _cameraFollow;

    private MinorCheckpoint _checkpoint;

    private TriggerProxy _tutorialTriggerProxy;
    private DialogueController _tutorialDialogueController;

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
        
        _curvedStarMaterial = transform.Find("Node").Find("CurvedStar").GetComponent<Renderer>().material;
        _channelHaloRenderer = transform.Find("Node").Find("ChannelHalo").GetComponent<Renderer>();
        _channelHaloMaterial = _channelHaloRenderer.material;
        _channelParticles = transform.Find("Node").Find("ChannelParticles").GetComponent<ParticleSystem>();
        _completedParticles = transform.Find("Node").Find("CompletedParticles").GetComponent<ParticleSystem>();
        _interactableLight = transform.Find("Node").Find("InteractableLight").GetComponent<Light>();
        
        _nodeBaseMaterial = transform.Find("Node").Find("major-leyline-node").GetComponent<Renderer>().material;

        _checkpoint = GetComponentInChildren<MinorCheckpoint>();

        _visualSplineMaterial.SetFloat("_FillWeight", 0f);
        _checkpoint.gameObject.SetActive(false);

        _tutorialTriggerProxy = GetComponentInChildren<TriggerProxy>();
        _tutorialDialogueController = transform.Find("TutorialDialogue").GetComponent<DialogueController>();
        
        if (previousNodeMetaName == "")
        {
            // _visualSplineContainer.gameObject.SetActive(false);
            _interactable.SetEnabled(true);
        }
        else
        {
            _interactable.SetEnabled(SaveSystem.GetMajorLeylineNode(previousNodeMetaName));

            if (!SaveSystem.GetMajorLeylineNode(previousNodeMetaName))
            {
                _interactableHaloRenderer.enabled = false;
                _channelHaloRenderer.enabled = false;
                _curvedStarMaterial.SetFloat("_Clip", 0);
            }
        }
        
        if (SaveSystem.GetMajorLeylineNode(metaName))
        {
            _interactable.SetEnabled(false);
            _interactable.enabled = false;
            _visualSplineMaterial.SetFloat("_FillWeight", 1f);

            _interactableHaloRenderer.enabled = false;
            _channelHaloRenderer.enabled = false;
            _curvedStarMaterial.SetFloat("_Clip", 1);
            _interactableLight.enabled = false;
            _nodeBaseMaterial.SetFloat("_CompletionWeight", 1f);
            _checkpoint.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
        SaveSystem.OnSaveDataUpdated += OnSaveDataUpdated;
        _tutorialTriggerProxy.OnTriggerProxyStay += OnTutorialTrigger;
    }

    private void OnTutorialTrigger(Collider obj)
    {
        //
        // DialogueCanvas.Singleton.StartDialogue(_tutorialDialogueController);
        // PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.Dialogue);
    }


    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
        SaveSystem.OnSaveDataUpdated -= OnSaveDataUpdated;
        _tutorialTriggerProxy.OnTriggerProxyStay -= OnTutorialTrigger;
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
        
        SaveSystem.WriteMajorLeylineNode(metaName);
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
                _curvedStarMaterial.SetFloat("_Clip", w);
                t += Time.deltaTime;
                yield return null;
            }

            _interactableHaloRenderer.enabled = false;
            yield return new WaitForSeconds(0.9f);
            _completedParticles.Play();
            _interactableLight.enabled = false;
            yield return new WaitForSeconds(1.1f);
            CutsceneManager.Singleton.SetPseudoCutsceneActive(true, _cameraLookAt);
            yield return new WaitForSeconds(0.5f);
            
            
            t = 0f;
            d = Mathf.Min(_visualSplineContainer.Spline.GetLength() * 0.03f, 7f);
            _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(0);
            _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(0);
            _virtualCamera.Priority = 20;

            _channelParticles.Stop();
            _completedParticles.Stop();

            var cameraFollowEndPosition = _cameraSplineContainer.EvaluatePosition(1);
            var finalDirection = PlayerFsm.Singleton.transform.position - new Vector3(cameraFollowEndPosition.x, cameraFollowEndPosition.y, cameraFollowEndPosition.z);
            PlayerCinemachineFreeLook.Singleton.OnPlayerCinemachineFreeLookScript(finalDirection, 0.5f);
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(t /
                    (d * cameraSplineFollowDurationMultiplier));
                _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(w);
                _visualSplineMaterial.SetFloat("_FillWeight", w);
                t += Time.deltaTime;
                yield return null;
            }
            

            _virtualCamera.Priority = -20;

            t = 0;
            d = 0.6f;

            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _nodeBaseMaterial.SetFloat("_CompletionWeight", w);
                t += Time.deltaTime;
                yield return null;
            }

            _checkpoint.gameObject.SetActive(true);
            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
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
            d = 0.75f;
            
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
            _interactable.enabled = true;
            
            _interactableLight.enabled = true;
            _interactableHaloRenderer.enabled = true;
            _channelHaloRenderer.enabled = true;
            _curvedStarMaterial.SetFloat("_Clip", 0);
        }
    }
}
