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
    private Material _material;

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

        _material = _visualSplineContainer.GetComponent<MeshRenderer>().material;
        _interactable = GetComponentInChildren<Interactable>();
        
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _virtualCamera.Priority = -20;

        if (previousNodeMetaName == "")
        {
            _visualSplineContainer.gameObject.SetActive(false);
        }
        else
        {
            _interactable.SetEnabled(SaveSystem.GetMajorLeylineNode(previousNodeMetaName));
        }
        
        if (SaveSystem.GetMajorLeylineNode(metaName))
        {
            _interactable.SetEnabled(false);
            _material.SetFloat("_FillWeight", 1f);
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
        if (previousNodeMetaName == "") return;
        var curveLength = _visualSplineContainer.Spline.GetLength();
        _material.SetFloat("_SplineLength", curveLength);
        
    }

    void OnInteracted()
    {
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.WalkToMajorLeylinePosition);
        PlayerFsm.Singleton.walkToPositionTarget = _interactable.transform.position;
        
        if (previousNodeMetaName == "")
        {
            
        }
        else
        {
            StartCoroutine(Coroutine());
        }
        SaveSystem.WriteMajorLeylineNode(metaName);
        _interactable.SetEnabled(false);

        IEnumerator Coroutine()
        {
            var t = 0f;
            var d = _visualSplineContainer.Spline.GetLength() * 0.03f;
            CutsceneManager.Singleton.SetPseudoCutsceneActive(true, _cameraLookAt);
            yield return new WaitForSeconds(3f);

            _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(0);
            _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(0);
            _virtualCamera.Priority = 20;
            yield return new WaitForSeconds(0.25f);

            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _cameraFollow.position = _cameraSplineContainer.EvaluatePosition(w);
                _cameraLookAt.position = _visualSplineContainer.EvaluatePosition(w);
                _material.SetFloat("_FillWeight", w);
                t += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(0.4f);
            
            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _virtualCamera.Priority = -20;
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
