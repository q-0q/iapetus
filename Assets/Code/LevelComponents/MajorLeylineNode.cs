using System;
using UnityEngine;
using UnityEngine.Splines;

public class MajorLeylineNode : MonoBehaviour
{
    private SplineContainer _splineContainer;
    private Interactable _interactable;
    
    private void Awake()
    {
        _splineContainer = GetComponentInChildren<SplineContainer>();
        _interactable = GetComponentInChildren<Interactable>();
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    void Start()
    {
        var curveLength = _splineContainer.Spline.GetLength();
        _splineContainer.GetComponent<MeshRenderer>().material.SetFloat("_SplineLength", curveLength);
        
    }

    void OnInteracted()
    {
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.MajorLeylineNodeInteract);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
